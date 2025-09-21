using System.Globalization;
using System.Numerics;
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

    private const int ChartMaxWindowHours = 24;

    private static readonly TimeSpan MinSplitWindow = TimeSpan.FromMinutes(30);

    private static bool IsQueryFileLimit(Exception ex)
    {
        var s = ex.ToString();
        return s.IndexOf("file limit", StringComparison.OrdinalIgnoreCase) >= 0
            || s.IndexOf("parquet files", StringComparison.OrdinalIgnoreCase) >= 0
            || s.IndexOf("Query would exceed file limit", StringComparison.OrdinalIgnoreCase) >= 0;
    }

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

        // ---- Aggregates (hourly mean anchored at the hour that contains the latest sample) ----
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

        // Latest per sensor by measurement type
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
                            Value = pv,
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

    // ---------------- Charts (daily means across selected sensors) ----------------

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
        // Accumulate per-day SUM and COUNT across windows, then compute final AVG
        var sums = new Dictionary<DateOnly, (double sum, long cnt)>();

        // Start with smaller initial window
        var initialWindow = TimeSpan.FromHours(ChartMaxWindowHours);
        var totalSpan = toExclusiveUtc - fromUtc;

        // If total span is large, start with even smaller windows
        if (totalSpan > TimeSpan.FromDays(7))
        {
            initialWindow = TimeSpan.FromHours(12);
        }

        if (totalSpan > TimeSpan.FromDays(14))
        {
            initialWindow = TimeSpan.FromHours(6);
        }

        await QueryDailyWindowOrSplitAsync(
                table,
                addrParams,
                baseNamed,
                fromUtc,
                toExclusiveUtc,
                initialWindow,
                sums,
                ct
            )
            .ConfigureAwait(false);

        var points = new List<MetricPointDto>();
        for (var d = reqFrom; d <= reqTo; d = d.AddDays(1))
        {
            if (sums.TryGetValue(d, out var sc) && sc.cnt > 0)
                points.Add(
                    new MetricPointDto
                    {
                        Date = d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        Value = sc.sum / sc.cnt,
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

    private async Task QueryDailyWindowOrSplitAsync(
        string table,
        string addrParams,
        IReadOnlyDictionary<string, object> baseNamed,
        DateTime winFromUtc,
        DateTime winToUtc,
        TimeSpan maxWindow,
        Dictionary<DateOnly, (double sum, long cnt)> acc,
        CancellationToken ct
    )
    {
        if (winFromUtc >= winToUtc)
            return;

        var currentSpan = winToUtc - winFromUtc;

        // Split if window is too large
        if (currentSpan > maxWindow)
        {
            var mid = winFromUtc + TimeSpan.FromTicks(currentSpan.Ticks / 2);
            await QueryDailyWindowOrSplitAsync(
                table,
                addrParams,
                baseNamed,
                winFromUtc,
                mid,
                maxWindow,
                acc,
                ct
            );
            await QueryDailyWindowOrSplitAsync(
                table,
                addrParams,
                baseNamed,
                mid,
                winToUtc,
                maxWindow,
                acc,
                ct
            );
            return;
        }

        var named = new Dictionary<string, object>(baseNamed)
        {
            ["from"] = winFromUtc.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            ["to"] = winToUtc.ToString("yyyy-MM-ddTHH:mm:ssZ"),
        };

        var sql =
            $@"
        SELECT DATE_TRUNC('day', time) AS d, SUM(value) AS s, COUNT(value) AS n
        FROM ""{table}""
        WHERE time >= $from AND time < $to
          AND source_address IN ({addrParams})
        GROUP BY 1
        ORDER BY 1";

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
                    var sum = ToDouble(row[1]);
                    var cnt = ToInt64(row[2]);

                    if (acc.TryGetValue(day, out var sc))
                        acc[day] = (sc.sum + sum, sc.cnt + cnt);
                    else
                        acc[day] = (sum, cnt);
                }
            }
        }
        catch (Exception ex) when (IsQueryFileLimit(ex))
        {
            // If we hit file limit, split further but ensure we don't go below minimum
            if (currentSpan <= MinSplitWindow)
            {
                // If we can't split further, try a different approach - skip this window
                // or throw with more context
                throw new InvalidOperationException(
                    $"Cannot split query window further. Current span: {currentSpan}, "
                        + $"minimum allowed: {MinSplitWindow}. Consider reducing the date range or "
                        + $"increasing the InfluxDB file limit.",
                    ex
                );
            }

            var mid = winFromUtc + TimeSpan.FromTicks(currentSpan.Ticks / 2);
            await QueryDailyWindowOrSplitAsync(
                table,
                addrParams,
                baseNamed,
                winFromUtc,
                mid,
                maxWindow,
                acc,
                ct
            );
            await QueryDailyWindowOrSplitAsync(
                table,
                addrParams,
                baseNamed,
                mid,
                winToUtc,
                maxWindow,
                acc,
                ct
            );
        }
    }

    private async Task<List<MetricPointDto>> QueryDailySeriesAsync(
        string table,
        string addrParams,
        IReadOnlyDictionary<string, object> baseNamed,
        DateOnly from,
        DateOnly to,
        CancellationToken ct
    )
    {
        var named = new Dictionary<string, object>(baseNamed);

        var sql =
            $@"
            SELECT DATE_TRUNC('day', time) AS d, AVG(value) AS v
            FROM ""{table}""
            WHERE time >= $from AND time < $to
              AND source_address IN ({addrParams})
            GROUP BY 1
            ORDER BY 1";

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
            // Fall back to adaptive approach
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

        // Fill entire requested range; null when no data for that day
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

    // ---------------- Summaries anchored to the *latest* hour ----------------

    private static readonly int[] LatestLookbackDays = { 7, 30, 90, 365, 730 };

    private async Task<MeasureValueDto> QueryLatestHourMeanAsync(
        string table,
        string unit,
        string addrParams,
        Dictionary<string, object> named,
        CancellationToken ct
    )
    {
        var toUtc = DateTime.UtcNow;
        var fromUtc = toUtc.AddYears(-2);

        // 1) Find latest timestamp adaptively
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

        // 2) Average within that hour only (tiny window => safe)
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
            Value = v,
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
        return await LatestTsWindowOrSplitAsync(
                table,
                addrParams,
                baseNamed,
                fromUtc,
                toUtc,
                TimeSpan.FromHours(ChartMaxWindowHours),
                ct
            )
            .ConfigureAwait(false);
    }

    private async Task<DateTime?> LatestTsWindowOrSplitAsync(
        string table,
        string addrParams,
        IReadOnlyDictionary<string, object> baseNamed,
        DateTime winFromUtc,
        DateTime winToUtc,
        TimeSpan maxWindow,
        CancellationToken ct
    )
    {
        if (winFromUtc >= winToUtc)
            return null;

        var span = winToUtc - winFromUtc;
        if (span > maxWindow)
        {
            var mid = winFromUtc + TimeSpan.FromTicks(span.Ticks / 2);
            var a = await LatestTsWindowOrSplitAsync(
                table,
                addrParams,
                baseNamed,
                winFromUtc,
                mid,
                maxWindow,
                ct
            );
            var b = await LatestTsWindowOrSplitAsync(
                table,
                addrParams,
                baseNamed,
                mid,
                winToUtc,
                maxWindow,
                ct
            );
            return new[] { a, b }.Max();
        }

        var named = new Dictionary<string, object>(baseNamed)
        {
            ["from"] = winFromUtc.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            ["to"] = winToUtc.ToString("yyyy-MM-ddTHH:mm:ssZ"),
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
            if (span <= MinSplitWindow)
                throw;
            var mid = winFromUtc + TimeSpan.FromTicks(span.Ticks / 2);
            var a = await LatestTsWindowOrSplitAsync(
                table,
                addrParams,
                baseNamed,
                winFromUtc,
                mid,
                maxWindow,
                ct
            );
            var b = await LatestTsWindowOrSplitAsync(
                table,
                addrParams,
                baseNamed,
                mid,
                winToUtc,
                maxWindow,
                ct
            );
            return new[] { a, b }.Max();
        }
    }

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
            _ => Convert.ToDouble(cell),
        };

    private sealed class LastRow
    {
        public DateTime Time { get; init; }
        public double Value { get; init; }
    }

    private async Task<Dictionary<string, LastRow>> QueryLatestPerSensorAsync(
        string table,
        string addrParams,
        Dictionary<string, object> named,
        CancellationToken ct
    )
    {
        var toUtc = DateTime.UtcNow;
        var fromUtc = toUtc.AddYears(-2);

        var acc = new Dictionary<string, LastRow>(StringComparer.OrdinalIgnoreCase);

        await LatestPerSensorWindowOrSplitAsync(
                table,
                addrParams,
                named,
                fromUtc,
                toUtc,
                TimeSpan.FromHours(ChartMaxWindowHours),
                acc,
                ct
            )
            .ConfigureAwait(false);

        return acc;
    }

    private async Task LatestPerSensorWindowOrSplitAsync(
        string table,
        string addrParams,
        IReadOnlyDictionary<string, object> baseNamed,
        DateTime winFromUtc,
        DateTime winToUtc,
        TimeSpan maxWindow,
        Dictionary<string, LastRow> acc,
        CancellationToken ct
    )
    {
        if (winFromUtc >= winToUtc)
            return;

        var span = winToUtc - winFromUtc;
        if (span > maxWindow)
        {
            var mid = winFromUtc + TimeSpan.FromTicks(span.Ticks / 2);
            await LatestPerSensorWindowOrSplitAsync(
                table,
                addrParams,
                baseNamed,
                winFromUtc,
                mid,
                maxWindow,
                acc,
                ct
            );
            await LatestPerSensorWindowOrSplitAsync(
                table,
                addrParams,
                baseNamed,
                mid,
                winToUtc,
                maxWindow,
                acc,
                ct
            );
            return;
        }

        var named = new Dictionary<string, object>(baseNamed)
        {
            ["from"] = winFromUtc.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            ["to"] = winToUtc.ToString("yyyy-MM-ddTHH:mm:ssZ"),
        };

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
              AND source_address IN ({addrParams})
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

                if (!acc.TryGetValue(addr, out var current) || ts > current.Time)
                    acc[addr] = new LastRow { Time = ts, Value = val };
            }
        }
        catch (Exception ex) when (IsQueryFileLimit(ex))
        {
            if (span <= MinSplitWindow)
                throw;
            var mid = winFromUtc + TimeSpan.FromTicks(span.Ticks / 2);
            await LatestPerSensorWindowOrSplitAsync(
                table,
                addrParams,
                baseNamed,
                winFromUtc,
                mid,
                maxWindow,
                acc,
                ct
            );
            await LatestPerSensorWindowOrSplitAsync(
                table,
                addrParams,
                baseNamed,
                mid,
                winToUtc,
                maxWindow,
                acc,
                ct
            );
        }
    }

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
