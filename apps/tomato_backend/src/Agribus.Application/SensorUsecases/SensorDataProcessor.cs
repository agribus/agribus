using Agribus.Core.Ports.Api.ParseSensorData;
using Agribus.Core.Ports.Api.ParseSensorData.DTOs;
using Agribus.Core.Ports.Spi.Measurement;

namespace Agribus.Application.SensorUsecases;

public class SensorDataProcessor(IParseSensorData parser, IStoreMeasurement store)
{
    public async Task ProcessAsync(
        RawSensorPayload raw,
        CancellationToken cancellationToken = default
    )
    {
        var parsed = await parser.FromRawJson(raw, cancellationToken);
        await store.StoreAsync(parsed, cancellationToken);
    }
}
