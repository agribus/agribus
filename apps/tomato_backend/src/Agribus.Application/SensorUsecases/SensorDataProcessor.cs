using Agribus.Core.Domain.AggregatesModels.SensorAggregates;
using Agribus.Core.Ports.Api.ParseSensorData;
using Agribus.Core.Ports.Api.ParseSensorData.DTOs;
using Agribus.Core.Ports.Spi.Measurement;
using Agribus.Core.Ports.Spi.SensorContext;

namespace Agribus.Application.SensorUsecases;

public class SensorDataProcessor(
    IParseSensorData parser,
    IStoreMeasurement store,
    ISensorRepository sensorRepository
)
{
    public async Task ProcessAsync(
        RawSensorPayload raw,
        CancellationToken cancellationToken = default
    )
    {
        if (!await sensorRepository.IsRegistered(raw.SourceAddress, cancellationToken))
            throw new UnregisteredSensorException(
                $"Sensor with address {raw.SourceAddress} is not registered"
            );
        var parsed = await parser.FromRawJson(raw, cancellationToken);
        await store.StoreAsync(parsed, cancellationToken);
    }

    public async Task<List<SensorMeasurement>> GetMeasurementsAsync(
        List<string> sourceAddresses,
        CancellationToken cancellationToken
    )
    {
        return await store.GetMeasurementsAsync(sourceAddresses, cancellationToken);
    }
}

public class UnregisteredSensorException(string message) : Exception(message);
