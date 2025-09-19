using System.Numerics;
using Agribus.Core.Domain.AggregatesModels.SensorAggregates;
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
            WritePrecision.S,
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

        // ---- Aggregates (hourly mean over last hour) ----
        resp.SummaryAggreglates.Metrics.Temperature = await QueryHourlyMeanAsync(
            table: "Temperature",
            unit: "°C",
            addrParams,
            named,
            cancellationToken
        );

        resp.SummaryAggreglates.Metrics.Humidity = await QueryHourlyMeanAsync(
            table: "Humidity",
            unit: "%",
            addrParams,
            named,
            cancellationToken
        );

        resp.SummaryAggreglates.Metrics.AirPressure = await QueryHourlyMeanAsync(
            table: "Pressure",
            unit: "hPa",
            addrParams,
            named,
            cancellationToken
        );

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

    private static DateTime? MaxNullable(params DateTime?[] values) =>
        values.Where(v => v.HasValue).Select(v => v!.Value).DefaultIfEmpty().Max();

    private static DateTime ToUtcTime(object cell) =>
        cell switch
        {
            DateTime dt => DateTime.SpecifyKind(dt, DateTimeKind.Utc),
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

    private async Task<MeasureValueDto> QueryHourlyMeanAsync(
        string table,
        string unit,
        string addrParams,
        Dictionary<string, object> named,
        CancellationToken ct
    )
    {
        // AVG over last hour across selected sensors; timestamp = hour of newest sample
        var sql =
            $@"
            SELECT
              AVG(value)                                   AS value,
              DATE_TRUNC('hour', MAX(time))               AS ts
            FROM ""{table}""
            WHERE time >= now() - INTERVAL '1 hour'
              AND source_address IN ({addrParams})";

        double? v = null;
        DateTime? ts = null;

        await foreach (
            var row in _client
                .Query(sql, database: _options.Bucket, namedParameters: named)
                .WithCancellation(ct)
        )
        {
            if (row is [not null, not null, ..])
            {
                v = ToDouble(row[0]);
                ts = ToUtcTime(row[1]);
            }

            break; // single row
        }

        return new MeasureValueDto
        {
            Value = v,
            Unit = unit,
            Timestamp = ts,
        };
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
        var sql =
            $@"
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
            WHERE rn = 1
        ";

        var dict = new Dictionary<string, LastRow>(StringComparer.OrdinalIgnoreCase);

        await foreach (
            var row in _client
                .Query(sql, database: _options.Bucket, namedParameters: named)
                .WithCancellation(ct)
        )
        {
            // source_address, value, time
            var addr = (string)row[0];
            var val = ToDouble(row[1]);
            var ts = ToUtcTime(row[2]);

            dict[addr] = new LastRow { Time = ts, Value = val };
        }

        return dict;
    }
}
