using Agribus.Core.Ports.Api.SensorUsecases;
using Agribus.Core.Ports.Api.SensorUsecases.DTOs;
using Agribus.Core.Ports.Spi.SensorContext;

namespace Agribus.Application.SensorUsecases;

public class UpdateSensorUsecase(ISensorRepository sensorRepository) : IUpdateSensorUsecase
{
    public async Task<bool?> Handle(
        Guid originalSensorId,
        string userId,
        UpdateSensorDto dto,
        CancellationToken cancellationToken
    )
    {
        var sensor = await sensorRepository.Exists(originalSensorId, userId, cancellationToken);
        if (sensor == null)
            return false;

        return await sensorRepository.UpdateAsync(sensor, dto, cancellationToken);
    }

    public async Task<bool?> HandleFromGreenhouse(
        string userId,
        UpdateSensorFromGreenhouseDto dto,
        CancellationToken cancellationToken
    )
    {
        var sensor = await sensorRepository.Exists(dto.Id, userId, cancellationToken);
        if (sensor == null)
            return null;

        return await sensorRepository.UpdateAsync(sensor, dto, cancellationToken);
    }
}
