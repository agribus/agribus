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
}
