using Agribus.Core.Domain.AggregatesModels;
using Agribus.Core.Ports.Spi.Measurement;
using InfluxDB.Client;
using Microsoft.Extensions.Options;

namespace Agribus.InfluxDB.Repositories;

public class InfluxMeasurementStore : IStoreMeasurement
{
    private readonly InfluxDBClient _client;
    private readonly InfluxOptions _options;

    public InfluxMeasurementStore(IOptions<InfluxOptions> options)
    {
        _options = options.Value;
        _client = new InfluxDBClient(_options.Url, _options.Token);
    }

    public Task StoreAsync(
        SensorMeasurement measurement,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException("InfluxMeasurementStore is not implemented");
    }
}
