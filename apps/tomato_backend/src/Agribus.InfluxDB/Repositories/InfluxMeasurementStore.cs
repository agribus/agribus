using Agribus.Core.Domain.AggregatesModels.SensorAggregates;
using Agribus.Core.Ports.Spi.Measurement;
using Agribus.InfluxDB.Mapping;
using InfluxDB.Client;
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
        await _client
            .GetWriteApiAsync()
            .WritePointAsync(pointData, _options.Bucket, _options.Org, cancellationToken);
    }

    public async Task<List<SensorMeasurement>> GetMeasurementsAsync(
        List<string> sourceAddresses,
        CancellationToken cancellationToken = default
    )
    {
        if (sourceAddresses.Count == 0)
        {
            return [];
        }

        var query = $@"from(bucket: ""{_options.Bucket}"")";
        query += " |> range(start: -90d, stop: now())";

        var sourceAddressConditions = sourceAddresses.Select(addr =>
            $@"r.source_address == ""{addr}"""
        );
        query += " |> filter(fn: (r) => " + string.Join(" or ", sourceAddressConditions) + ")";

        query += " |> filter(fn: (r) => r._field == \"value\")";
        query += " |> sort(columns: [\"_time\"], desc: false)";

        var tables = await _client.GetQueryApi().QueryAsync(query, _options.Org, cancellationToken);

        return (
            from table in tables
            from record in table.Records
            select record.ToSensorMeasurement()
        ).ToList();
    }
}
