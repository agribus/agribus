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
        var tTemp = QueryLatestHourMeanAsync("Temperature", "°C", addrParams, named, cancellationToken);
        var tHum = QueryLatestHourMeanAsync("Humidity", "%", addrParams, named, cancellationToken);
        var tPres = QueryLatestHourMeanAsync("Pressure", "hPa", addrParams, named, cancellationToken);

        await Task.WhenAll(tTemp, tHum, tPres);

        resp.SummaryAggreglates.Metrics.Temperature = await tTemp;
        resp.SummaryAggreglates.Metrics.Humidity = await tHum;
        resp.SummaryAggreglates.Metrics.AirPressure = await tPres;

        // Latest per sensor by measurement type
        var latestTemp = await QueryLatestPerSensorAsync("Temperature", addrParams, named, cancellationToken);
        var latestHum = await QueryLatestPerSensorAsync("Humidity", addrParams, named, cancellationToken);
        var latestPress = await QueryLatestPerSensorAsync("Pressure", addrParams, named, cancellationToken);

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
                        ? new MeasureValueDto { Value = tv, Unit = "°C", Timestamp = t!.Time }
                        : null,
                    Humidity = h?.Value is { } hv
                        ? new MeasureValueDto { Value = hv, Unit = "%", Timestamp = h!.Time }
                        : null,
                    Pressure = p?.Value is { } pv
                        ? new MeasureValueDto { Value = pv, Unit = "hPa", Timestamp = p!.Time }
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

        // Inclusive [from, to] UTC -> [from 00:00Z, to+1d 00:00Z)
        var fromUtc = DateTime.SpecifyKind(from.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        var toExclusiveUtc = DateTime.SpecifyKind(to.AddDays(1).ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);

        // Influx SQL named parameters must be simple types; use RFC3339 strings
        baseNamed["from"] = fromUtc.ToString("yyyy-MM-ddTHH:mm:ssZ");
        baseNamed["to"] = toExclusiveUtc.ToString("yyyy-MM-ddTHH:mm:ssZ");

        var tempTask = QueryDailySeriesAsync("Temperature", addrParams, baseNamed, from, to, ct);
        var humTask = QueryDailySeriesAsync("Humidity", addrParams, baseNamed, from, to, ct);
        var pressTask = QueryDailySeriesAsync("Pressure", addrParams, baseNamed, from, to, ct);

        await Task.WhenAll(tempTask, humTask, pressTask).ConfigureAwait(false);

        resp.Metrics.Temperature.Points = tempTask.Result;
        resp.Metrics.Humidity.Points = humTask.Result;
        resp.Metrics.AirPressure.Points = pressTask.Result;

        return resp;
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
        // Clone to be safe if the caller reuses baseNamed concurrently
        var named = new Dictionary<string, object>(baseNamed);

        var sql = $@"
            SELECT DATE_TRUNC('day', time) AS d, AVG(value) AS v
            FROM ""{table}""
            WHERE time >= $from AND time < $to
              AND source_address IN ({addrParams})
            GROUP BY 1
            ORDER BY 1";

        var byDay = new Dictionary<DateOnly, double>();

        await foreach (var row in _client.Query(sql, database: _options.Bucket, namedParameters: named).WithCancellation(ct))
        {
            if (row is [not null, not null, ..])
            {
                var day = DateOnly.FromDateTime(ToUtcTime(row[0]));
                var val = ToDouble(row[1]);
                byDay[day] = val;
            }
        }

        // Fill entire requested range; null when no data for that day
        var points = new List<MetricPointDto>();
        for (var d = from; d <= to; d = d.AddDays(1))
        {
            var has = byDay.TryGetValue(d, out var v);
            points.Add(new MetricPointDto
            {
                Date = d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                Value = has ? v : (double?)null
            });
        }

        return points;
    }

    private static List<MetricPointDto> GenerateEmptyPoints(DateOnly from, DateOnly to)
    {
        var list = new List<MetricPointDto>();
        for (var d = from; d <= to; d = d.AddDays(1))
        {
            list.Add(new MetricPointDto
            {
                Date = d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                Value = null
            });
        }
        return list;
    }

    // ---------------- Summaries anchored to the *latest* hour ----------------

    private async Task<MeasureValueDto> QueryLatestHourMeanAsync(
        string table,
        string unit,
        string addrParams,
        Dictionary<string, object> named,
        CancellationToken ct
    )
    {
        // Hour bucket containing the most recent sample across selected sensors
        var sql = $@"
            WITH last_ts AS (
                SELECT MAX(time) AS t
                FROM ""{table}""
                WHERE source_address IN ({addrParams})
            ),
            bounds AS (
                SELECT
                    DATE_TRUNC('hour', t)                     AS start_ts,
                    DATE_TRUNC('hour', t) + INTERVAL '1 hour' AS end_ts
                FROM last_ts
            )
            SELECT
                AVG(m.value) AS value,
                b.start_ts   AS ts
            FROM ""{table}"" AS m
            CROSS JOIN bounds AS b
            WHERE m.source_address IN ({addrParams})
              AND m.time >= b.start_ts
              AND m.time <  b.end_ts
            GROUP BY b.start_ts";

        double? v = null;
        DateTime? ts = null;

        await foreach (var row in _client.Query(sql, database: _options.Bucket, namedParameters: named).WithCancellation(ct))
        {
            if (row is [not null, not null, ..])
            {
                v = ToDouble(row[0]);
                ts = ToUtcTime(row[1]); // hour start (UTC) of the chosen bucket
            }
            break; // single row expected
        }

        return new MeasureValueDto { Value = v, Unit = unit, Timestamp = ts };
    }

    // ---------------- Common helpers ----------------

    private static DateTime? MaxNullable(params DateTime?[] values) =>
        values.Where(v => v.HasValue).Select(v => v!.Value).DefaultIfEmpty().Max();

    private static DateTime ToUtcTime(object cell) =>
        cell switch
        {
            DateTime dt => DateTime.SpecifyKind(dt, DateTimeKind.Utc),
            DateTimeOffset dto => dto.UtcDateTime,
            string s => DateTimeOffset.Parse(
                                    s,
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal
                                  ).UtcDateTime,
            BigInteger bi => DateTimeOffset.FromUnixTimeMilliseconds((long)(bi / 1_000_000)).UtcDateTime, // ns -> ms
            long ms => DateTimeOffset.FromUnixTimeMilliseconds(ms).UtcDateTime,
            double dms => DateTimeOffset.FromUnixTimeMilliseconds((long)dms).UtcDateTime,
            _ => throw new InvalidCastException($"Unsupported time type: {cell.GetType().FullName}")
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
            _ => Convert.ToDouble(cell)
        };

    // (Optional) Kept for reference; not used by current flow
    private async Task<MeasureValueDto> QueryHourlyMeanAsync(
        string table,
        string unit,
        string addrParams,
        Dictionary<string, object> named,
        CancellationToken ct
    )
    {
        var sql = $@"
            SELECT
              AVG(value)                         AS value,
              DATE_TRUNC('hour', MAX(time))      AS ts
            FROM ""{table}""
            WHERE time >= now() - INTERVAL '1 hour'
              AND source_address IN ({addrParams})";

        double? v = null;
        DateTime? ts = null;

        await foreach (var row in _client.Query(sql, database: _options.Bucket, namedParameters: named).WithCancellation(ct))
        {
            if (row is [not null, not null, ..])
            {
                v = ToDouble(row[0]);
                ts = ToUtcTime(row[1]);
            }
            break; // single row
        }

        return new MeasureValueDto { Value = v, Unit = unit, Timestamp = ts };
    }

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
        var sql = $@"
            WITH ranked AS (
                SELECT
                    source_address,
                    value,
                    time,
                    ROW_NUMBER() OVER (PARTITION BY source_address ORDER BY time DESC) AS rn
                FROM ""{table}""
                WHERE source_address IN ({addrParams})
            )
            SELECT source_address, value, time
            FROM ranked
            WHERE rn = 1";

        var dict = new Dictionary<string, LastRow>(StringComparer.OrdinalIgnoreCase);

        await foreach (var row in _client.Query(sql, database: _options.Bucket, namedParameters: named).WithCancellation(ct))
        {
            var addr = (string)row[0];
            var val = ToDouble(row[1]);
            var ts = ToUtcTime(row[2]);
            dict[addr] = new LastRow { Time = ts, Value = val };
        }

        return dict;
    }
}
