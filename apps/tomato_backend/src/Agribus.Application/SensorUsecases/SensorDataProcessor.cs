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

    public async Task<LatestMeasurementsResponseDto> GetMeasurementsAsync(
        List<Sensor> sensors,
        CancellationToken cancellationToken
    )
    {
        return await store.GetMeasurementsAsync(sensors, cancellationToken);
    }

    public async Task<ChartTimeseriesResponseDto> GetChartTimeseriesAsync(
        List<Sensor> sensors,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default
    )
    {
        return await store.GetChartTimeseriesAsync(sensors, from, to, cancellationToken);
    }
}

public class UnregisteredSensorException(string message) : Exception(message);
