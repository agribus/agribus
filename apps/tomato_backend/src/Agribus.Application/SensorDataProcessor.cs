using Agribus.Core.Ports.Api.DTOs;
using Agribus.Core.Ports.Api.ParseSensorData;
using Agribus.Core.Ports.Spi.Measurement;

namespace Agribus.Application;

public class SensorDataProcessor
{
    private readonly IParseSensorData _parser;
    private readonly IStoreMeasurement _store;

    public SensorDataProcessor(IParseSensorData parser, IStoreMeasurement store)
    {
        _parser = parser;
        _store = store;
    }

    public async Task ProcessAsync(
        RawSensorPayload raw,
        CancellationToken cancellationToken = default
    )
    {
        var parsed = await _parser.FromRawJson(raw, cancellationToken);
        await _store.StoreAsync(parsed, cancellationToken);
    }
}
