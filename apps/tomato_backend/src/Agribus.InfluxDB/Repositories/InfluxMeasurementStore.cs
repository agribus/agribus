using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Threading;
using Agribus.Core.Domain.AggregatesModels.SensorAggregates;
using Agribus.Core.Ports.Api.ParseSensorData.DTOs;
using Agribus.Core.Ports.Spi.Measurement;
using Agribus.InfluxDB.Mapping;
using InfluxDB3.Client;
using InfluxDB3.Client.Write;
using Microsoft.Extensions.Options;

namespace Agribus.InfluxDB.Repositories;

public class InfluxMeasurementStore : IStoreMeasurement
{
    private readonly InfluxDBClient _client;
    private readonly InfluxOptions _options;

    public InfluxMeasurementStore(IOptions<InfluxOptions> options, InfluxDBClient client)
    {
        _options = options.Value;
        _client = client;
    }

    private const int ChartMaxWindowHours = 24; // <= 24h par fenêtre
    private const int ChartWindowDop = 3; // Degré de parallélisme pour les fenêtres chart
    private const int LatestHorizonYears = 2; // Jusqu’où chercher du “latest”
    private static readonly TimeSpan MinSplitWindow = TimeSpan.FromMinutes(30);

    private static bool IsQueryFileLimit(Exception ex)
    {
        var s = ex.ToString();
        return s.IndexOf("file limit", StringComparison.OrdinalIgnoreCase) >= 0
            || s.IndexOf("parquet files", StringComparison.OrdinalIgnoreCase) >= 0
            || s.IndexOf("Query would exceed file limit", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static IEnumerable<(DateTime From, DateTime To)> EnumerateForwardWindows(
        DateTime fromUtc,
        DateTime toUtc,
        TimeSpan size
    )
    {
        for (var s = fromUtc; s < toUtc; s = s.Add(size))
        {
            var e = s + size;
            if (e > toUtc)
                e = toUtc;
            yield return (s, e);
        }
    }

    private static async Task ForEachAsync<T>(
        IEnumerable<T> source,
        int degreeOfParallelism,
        Func<T, Task> body,
        CancellationToken ct
    )
    {
        using var sem = new SemaphoreSlim(degreeOfParallelism);
        var tasks = new List<Task>();

        foreach (var item in source)
        {
            await sem.WaitAsync(ct).ConfigureAwait(false);
            tasks.Add(
                Task.Run(
                    async () =>
                    {
                        try
                        {
                            await body(item).ConfigureAwait(false);
                        }
                        finally
                        {
                            sem.Release();
                        }
                    },
                    ct
                )
            );
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    private static (string inClause, Dictionary<string, object> named) BuildAddrParams(
        IEnumerable<string> addrs
    )
    {
        var named = new Dictionary<string, object>(StringComparer.Ordinal);
        var i = 0;
        var parts = new List<string>();
        foreach (var a in addrs)
        {
            var k = $"addr{i++}";
            named[k] = a;
            parts.Add($"${k}");
        }
        return (string.Join(", ", parts), named);
    }

    // ---------- Write ----------

    public async Task StoreAsync(
        SensorMeasurement measurement,
        CancellationToken cancellationToken = default
    )
    {
        var pointData = measurement.ToPointData();
        await _client.WritePointAsync(
            pointData,
            _options.Bucket,
            WritePrecision.Ms,
            cancellationToken: cancellationToken
        );
    }

    // ---------- Latest + résumé cartes ----------

    public async Task<LatestMeasurementsResponseDto> GetMeasurementsAsync(
        List<Sensor>? sensors,
        CancellationToken cancellationToken = default
    )
    {
        var resp = new LatestMeasurementsResponseDto();

        if (sensors is null || sensors.Count == 0)
            return resp;

        var addrParams = string.Join(", ", sensors.Select((_, i) => $"$addr{i}"));
        var named = new Dictionary<string, object>();
        for (var i = 0; i < sensors.Count; i++)
            named[$"addr{i}"] = sensors[i].SourceAddress;

        // Agrégats (moyenne sur l’heure qui contient le dernier échantillon global)
        var tTemp = QueryLatestHourMeanAsync(
            "Temperature",
            "°C",
            addrParams,
            named,
            cancellationToken
        );
        var tHum = QueryLatestHourMeanAsync("Humidity", "%", addrParams, named, cancellationToken);
        var tPres = QueryLatestHourMeanAsync(
            "Pressure",
            "hPa",
            addrParams,
            named,
            cancellationToken
        );

        await Task.WhenAll(tTemp, tHum, tPres);

        resp.SummaryAggreglates.Metrics.Temperature = await tTemp;
        resp.SummaryAggreglates.Metrics.Humidity = await tHum;
        resp.SummaryAggreglates.Metrics.AirPressure = await tPres;

        // Dernière mesure par capteur / type
        var latestTemp = await QueryLatestPerSensorAsync(
            "Temperature",
            addrParams,
            named,
            cancellationToken
        );
        var latestHum = await QueryLatestPerSensorAsync(
            "Humidity",
            addrParams,
            named,
            cancellationToken
        );
        var latestPress = await QueryLatestPerSensorAsync(
            "Pressure",
            addrParams,
            named,
            cancellationToken
        );

        foreach (var sensor in sensors)
        {
            latestTemp.TryGetValue(sensor.SourceAddress, out var t);
            latestHum.TryGetValue(sensor.SourceAddress, out var h);
            latestPress.TryGetValue(sensor.SourceAddress, out var p);

            var ts = MaxNullable(t?.Time, h?.Time, p?.Time);

            var card = new SensorCardDto
            {
                SourceAddress = sensor.SourceAddress,
                Name = sensor.Name,
                Last = new SensorLastDto
                {
                    Timestamp = ts,
                    Temperature = t?.Value is { } tv
                        ? new MeasureValueDto
                        {
                            Value = tv,
                            Unit = "°C",
                            Timestamp = t!.Time,
                        }
                        : null,
                    Humidity = h?.Value is { } hv
                        ? new MeasureValueDto
                        {
                            Value = hv,
                            Unit = "%",
                            Timestamp = h!.Time,
                        }
                        : null,
                    Pressure = p?.Value is { } pv
                        ? new MeasureValueDto
                        {
                            Value = pv / 100,
                            Unit = "hPa",
                            Timestamp = p!.Time,
                        }
                        : null,
                },
            };

            resp.Sensors.Add(card);
        }

        return resp;
    }

    // ---------- Charts (moyenne journalière sur la plage demandée) ----------

    public async Task<ChartTimeseriesResponseDto> GetChartTimeseriesAsync(
        List<Sensor> sensors,
        DateOnly from,
        DateOnly to,
        CancellationToken ct = default
    )
    {
        if (from > to)
            throw new ArgumentException("'from' must be <= 'to'.");

        var resp = new ChartTimeseriesResponseDto
        {
            Range = new ChartRangeDto
            {
                From = from.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                To = to.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            },
        };

        if (sensors is null || sensors.Count == 0)
        {
            resp.Metrics.Temperature.Points = GenerateEmptyPoints(from, to);
            resp.Metrics.Humidity.Points = GenerateEmptyPoints(from, to);
            resp.Metrics.AirPressure.Points = GenerateEmptyPoints(from, to);
            return resp;
        }

        var addrParams = string.Join(", ", sensors.Select((_, i) => $"$addr{i}"));
        var baseNamed = new Dictionary<string, object>(StringComparer.Ordinal);
        for (var i = 0; i < sensors.Count; i++)
            baseNamed[$"addr{i}"] = sensors[i].SourceAddress;

        var fromUtc = DateTime.SpecifyKind(from.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        var toExclusiveUtc = DateTime.SpecifyKind(
            to.AddDays(1).ToDateTime(TimeOnly.MinValue),
            DateTimeKind.Utc
        );

        var sw = Stopwatch.StartNew();

        var tempTask = QueryDailySeriesAdaptiveAsync(
            "Temperature",
            addrParams,
            baseNamed,
            from,
            to,
            fromUtc,
            toExclusiveUtc,
            ct
        );
        var humTask = QueryDailySeriesAdaptiveAsync(
            "Humidity",
            addrParams,
            baseNamed,
            from,
            to,
            fromUtc,
            toExclusiveUtc,
            ct
        );
        var pressTask = QueryDailySeriesAdaptiveAsync(
            "Pressure",
            addrParams,
            baseNamed,
            from,
            to,
            fromUtc,
            toExclusiveUtc,
            ct
        );

        await Task.WhenAll(tempTask, humTask, pressTask).ConfigureAwait(false);

        resp.Metrics.Temperature.Points = tempTask.Result;
        resp.Metrics.Humidity.Points = humTask.Result;
        resp.Metrics.AirPressure.Points = pressTask.Result;

        sw.Stop();
        // TODO: brancher un ILogger si besoin
        // _logger?.LogDebug("Charts range {From}->{To} in {Ms} ms", resp.Range.From, resp.Range.To, sw.ElapsedMilliseconds);

        return resp;
    }

    private async Task<List<MetricPointDto>> QueryDailySeriesAdaptiveAsync(
        string table,
        string addrParams,
        IReadOnlyDictionary<string, object> baseNamed,
        DateOnly reqFrom,
        DateOnly reqTo,
        DateTime fromUtc,
        DateTime toExclusiveUtc,
        CancellationToken ct
    )
    {
        var sums = new ConcurrentDictionary<DateOnly, (double Sum, long Cnt)>();

        var initial = TimeSpan.FromHours(ChartMaxWindowHours);
        var windows = EnumerateForwardWindows(fromUtc, toExclusiveUtc, initial).ToArray();

        async Task QueryWindowOrSplitAsync(DateTime wFrom, DateTime wTo)
        {
            var named = new Dictionary<string, object>(baseNamed)
            {
                ["from"] = wFrom.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                ["to"] = wTo.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            };

            var sql =
                $@"
                SELECT DATE_TRUNC('day', time) AS d, SUM(value) AS s, COUNT(value) AS n
                FROM ""{table}""
                WHERE time >= $from AND time < $to
                  AND source_address IN ({addrParams})
                GROUP BY 1"; // pas d'ORDER BY (inutile pour agrégats)

            try
            {
                await foreach (
                    var row in _client
                        .Query(sql, database: _options.Bucket, namedParameters: named)
                        .WithCancellation(ct)
                )
                {
                    if (row is [not null, not null, not null, ..])
                    {
                        var day = DateOnly.FromDateTime(ToUtcTime(row[0]));
                        var s = ToDouble(row[1]);
                        var n = ToInt64(row[2]);
                        sums.AddOrUpdate(day, (s, n), (_, prev) => (prev.Sum + s, prev.Cnt + n));
                    }
                }
            }
            catch (Exception ex) when (IsQueryFileLimit(ex))
            {
                var span = wTo - wFrom;
                if (span <= MinSplitWindow)
                    throw;

                var mid = wFrom + TimeSpan.FromTicks(span.Ticks / 2);
                await QueryWindowOrSplitAsync(wFrom, mid).ConfigureAwait(false);
                await QueryWindowOrSplitAsync(mid, wTo).ConfigureAwait(false);
            }
        }

        await ForEachAsync(windows, ChartWindowDop, w => QueryWindowOrSplitAsync(w.From, w.To), ct)
            .ConfigureAwait(false);

        var points = new List<MetricPointDto>(capacity: (reqTo.DayNumber - reqFrom.DayNumber) + 1);
        for (var d = reqFrom; d <= reqTo; d = d.AddDays(1))
        {
            if (sums.TryGetValue(d, out var sc) && sc.Cnt > 0)
                points.Add(
                    new MetricPointDto
                    {
                        Date = d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        Value = table == "Pressure" ? (sc.Sum / sc.Cnt) / 100 : sc.Sum / sc.Cnt,
                    }
                );
            else
                points.Add(
                    new MetricPointDto
                    {
                        Date = d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        Value = null,
                    }
                );
        }

        return points;
    }

    // Version simple (non utilisée par défaut) – garde comme fallback/lecture
    private async Task<List<MetricPointDto>> QueryDailySeriesAsync(
        string table,
        string addrParams,
        IReadOnlyDictionary<string, object> baseNamed,
        DateOnly from,
        DateOnly to,
        CancellationToken ct
    )
    {
        var named = new Dictionary<string, object>(baseNamed)
        {
            ["from"] = DateTime
                .SpecifyKind(from.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc)
                .ToString("yyyy-MM-ddTHH:mm:ssZ"),
            ["to"] = DateTime
                .SpecifyKind(to.AddDays(1).ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc)
                .ToString("yyyy-MM-ddTHH:mm:ssZ"),
        };

        var sql =
            $@"
            SELECT DATE_TRUNC('day', time) AS d, AVG(value) AS v
            FROM ""{table}""
            WHERE time >= $from AND time < $to
              AND source_address IN ({addrParams})
            GROUP BY 1";

        var byDay = new Dictionary<DateOnly, double>();

        try
        {
            await foreach (
                var row in _client
                    .Query(sql, database: _options.Bucket, namedParameters: named)
                    .WithCancellation(ct)
            )
            {
                if (row is [not null, not null, ..])
                {
                    var day = DateOnly.FromDateTime(ToUtcTime(row[0]));
                    var val = ToDouble(row[1]);
                    byDay[day] = val;
                }
            }
        }
        catch (Exception ex) when (IsQueryFileLimit(ex))
        {
            // Fallback sur la version adaptive parallélisée
            return await QueryDailySeriesAdaptiveAsync(
                table,
                addrParams,
                baseNamed,
                from,
                to,
                DateTime.SpecifyKind(from.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc),
                DateTime.SpecifyKind(to.AddDays(1).ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc),
                ct
            );
        }

        var points = new List<MetricPointDto>();
        for (var d = from; d <= to; d = d.AddDays(1))
        {
            var has = byDay.TryGetValue(d, out var v);
            points.Add(
                new MetricPointDto
                {
                    Date = d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Value = has ? v : (double?)null,
                }
            );
        }

        return points;
    }

    private static List<MetricPointDto> GenerateEmptyPoints(DateOnly from, DateOnly to)
    {
        var list = new List<MetricPointDto>();
        for (var d = from; d <= to; d = d.AddDays(1))
        {
            list.Add(
                new MetricPointDto
                {
                    Date = d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Value = null,
                }
            );
        }
        return list;
    }

    // ---------- Résumés ancrés sur la dernière heure contenant un point ----------

    private static readonly TimeSpan[] LatestProbeSpans = new[]
    {
        TimeSpan.FromHours(24),
        TimeSpan.FromDays(3),
        TimeSpan.FromDays(7),
        TimeSpan.FromDays(30),
        TimeSpan.FromDays(90),
        TimeSpan.FromDays(180),
        TimeSpan.FromDays(365),
        TimeSpan.FromDays(365 * LatestHorizonYears),
    };

    private async Task<MeasureValueDto> QueryLatestHourMeanAsync(
        string table,
        string unit,
        string addrParams,
        Dictionary<string, object> named,
        CancellationToken ct
    )
    {
        var toUtc = DateTime.UtcNow;
        var fromUtc = toUtc.AddYears(-LatestHorizonYears);

        // 1) Localiser le dernier timestamp via sondes progressives
        var latestTs = await GetLatestTimestampAdaptiveAsync(
                table,
                addrParams,
                named,
                fromUtc,
                toUtc,
                ct
            )
            .ConfigureAwait(false);

        if (latestTs is null)
            return new MeasureValueDto
            {
                Value = null,
                Unit = unit,
                Timestamp = null,
            };

        // 2) Moyenne dans l’heure (fenêtre minuscule => sûr et rapide)
        var hourStart = new DateTime(
            latestTs.Value.Year,
            latestTs.Value.Month,
            latestTs.Value.Day,
            latestTs.Value.Hour,
            0,
            0,
            DateTimeKind.Utc
        );
        var hourEnd = hourStart.AddHours(1);

        var np = new Dictionary<string, object>(named)
        {
            ["from"] = hourStart.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            ["to"] = hourEnd.ToString("yyyy-MM-ddTHH:mm:ssZ"),
        };

        var sql =
            $@"
            SELECT AVG(value) AS v
            FROM ""{table}""
            WHERE time >= $from AND time < $to
              AND source_address IN ({addrParams})";

        double? v = null;

        await foreach (
            var row in _client
                .Query(sql, database: _options.Bucket, namedParameters: np)
                .WithCancellation(ct)
        )
        {
            if (row is [not null, ..])
                v = ToDouble(row[0]);
            break;
        }

        return new MeasureValueDto
        {
            Value = (table == "Pressure" ? v / 100 : v),
            Unit = unit,
            Timestamp = hourStart,
        };
    }

    private async Task<DateTime?> GetLatestTimestampAdaptiveAsync(
        string table,
        string addrParams,
        IReadOnlyDictionary<string, object> baseNamed,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken ct
    )
    {
        var end = toUtc;
        foreach (var span in LatestProbeSpans)
        {
            var start = end - span;
            if (start < fromUtc)
                start = fromUtc;

            var ts = await MaxTimeInRangeWithSplitAsync(
                    table,
                    addrParams,
                    baseNamed,
                    start,
                    end,
                    ct
                )
                .ConfigureAwait(false);
            if (ts.HasValue)
                return ts;
            if (start <= fromUtc)
                break;
        }
        return null;
    }

    private async Task<DateTime?> MaxTimeInRangeWithSplitAsync(
        string table,
        string addrParams,
        IReadOnlyDictionary<string, object> baseNamed,
        DateTime wFromUtc,
        DateTime wToUtc,
        CancellationToken ct
    )
    {
        async Task<DateTime?> RunAsync(DateTime a, DateTime b)
        {
            var named = new Dictionary<string, object>(baseNamed)
            {
                ["from"] = a.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                ["to"] = b.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            };

            var sql =
                $@"
                SELECT MAX(time) AS t
                FROM ""{table}""
                WHERE time >= $from AND time < $to
                  AND source_address IN ({addrParams})";

            try
            {
                await foreach (
                    var row in _client
                        .Query(sql, database: _options.Bucket, namedParameters: named)
                        .WithCancellation(ct)
                )
                {
                    if (row is [not null, ..])
                        return ToUtcTime(row[0]);
                    break;
                }
                return null;
            }
            catch (Exception ex) when (IsQueryFileLimit(ex))
            {
                var span = b - a;
                if (span <= MinSplitWindow)
                    throw;

                var mid = a + TimeSpan.FromTicks(span.Ticks / 2);
                var left = await RunAsync(a, mid).ConfigureAwait(false);
                var right = await RunAsync(mid, b).ConfigureAwait(false);
                return new[] { left, right }.Max();
            }
        }

        return await RunAsync(wFromUtc, wToUtc).ConfigureAwait(false);
    }

    // ---------- Latest per sensor (scan arrière capteurs restants uniquement) ----------

    private sealed class LastRow
    {
        public DateTime Time { get; init; }
        public double Value { get; init; }
    }

    private async Task<Dictionary<string, LastRow>> QueryLatestPerSensorAsync(
        string table,
        string addrParamsAll,
        Dictionary<string, object> namedAll,
        CancellationToken ct
    )
    {
        var allAddrs = namedAll
            .Where(kv => kv.Key.StartsWith("addr", StringComparison.OrdinalIgnoreCase))
            .OrderBy(kv => kv.Key)
            .Select(kv => (string)kv.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var remaining = new HashSet<string>(allAddrs, StringComparer.OrdinalIgnoreCase);
        var result = new Dictionary<string, LastRow>(StringComparer.OrdinalIgnoreCase);

        var toUtc = DateTime.UtcNow;
        var fromUtc = toUtc.AddYears(-LatestHorizonYears);

        var chunk = TimeSpan.FromHours(ChartMaxWindowHours);
        var wEnd = toUtc;

        while (wEnd > fromUtc && remaining.Count > 0)
        {
            var wStart = wEnd - chunk;
            if (wStart < fromUtc)
                wStart = fromUtc;

            var (inClause, named) = BuildAddrParams(remaining);
            named["from"] = wStart.ToString("yyyy-MM-ddTHH:mm:ssZ");
            named["to"] = wEnd.ToString("yyyy-MM-ddTHH:mm:ssZ");

            var sql =
                $@"
                WITH ranked AS (
                    SELECT
                        source_address,
                        value,
                        time,
                        ROW_NUMBER() OVER (PARTITION BY source_address ORDER BY time DESC) AS rn
                    FROM ""{table}""
                    WHERE time >= $from AND time < $to
                      AND source_address IN ({inClause})
                )
                SELECT source_address, value, time
                FROM ranked
                WHERE rn = 1";

            try
            {
                await foreach (
                    var row in _client
                        .Query(sql, database: _options.Bucket, namedParameters: named)
                        .WithCancellation(ct)
                )
                {
                    var addr = (string)row[0];
                    var val = ToDouble(row[1]);
                    var ts = ToUtcTime(row[2]);

                    if (!result.TryGetValue(addr, out var cur) || ts > cur.Time)
                        result[addr] = new LastRow { Time = ts, Value = val };
                }

                // On retire les capteurs résolus
                foreach (var found in result.Keys.ToArray())
                    remaining.Remove(found);

                // Fenêtre suivante (on recule)
                wEnd = wStart;

                // on peut ré-élargir si on avait shrink précédemment (facultatif)
                if (chunk < TimeSpan.FromHours(ChartMaxWindowHours))
                    chunk = TimeSpan.FromHours(ChartMaxWindowHours);
            }
            catch (Exception ex) when (IsQueryFileLimit(ex))
            {
                if (chunk > MinSplitWindow)
                {
                    chunk = TimeSpan.FromTicks(Math.Max(MinSplitWindow.Ticks, chunk.Ticks / 2));
                    continue; // on réessaie la même position avec une fenêtre plus petite
                }
                throw; // irrécupérable
            }
        }

        return result;
    }

    // ---------- Conversions & helpers communs ----------

    private static DateTime? MaxNullable(params DateTime?[] values) =>
        values.Where(v => v.HasValue).Select(v => v!.Value).DefaultIfEmpty().Max();

    private static DateTime ToUtcTime(object cell) =>
        cell switch
        {
            DateTime dt => DateTime.SpecifyKind(dt, DateTimeKind.Utc),
            DateTimeOffset dto => dto.UtcDateTime,
            string s => DateTimeOffset
                .Parse(
                    s,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal
                )
                .UtcDateTime,
            BigInteger bi => DateTimeOffset
                .FromUnixTimeMilliseconds((long)(bi / 1_000_000))
                .UtcDateTime, // ns -> ms
            long ms => DateTimeOffset.FromUnixTimeMilliseconds(ms).UtcDateTime,
            double dms => DateTimeOffset.FromUnixTimeMilliseconds((long)dms).UtcDateTime,
            _ => throw new InvalidCastException(
                $"Unsupported time type: {cell.GetType().FullName}"
            ),
        };

    private static double ToDouble(object cell) =>
        cell switch
        {
            double d => d,
            float f => f,
            long l => l,
            int i => i,
            decimal m => (double)m,
            BigInteger b => (double)b,
            _ => Convert.ToDouble(cell, CultureInfo.InvariantCulture),
        };

    private static long ToInt64(object cell) =>
        cell switch
        {
            long l => l,
            int i => i,
            BigInteger bi => (long)bi,
            double d => (long)d,
            decimal m => (long)m,
            _ => Convert.ToInt64(cell, CultureInfo.InvariantCulture),
        };
}
