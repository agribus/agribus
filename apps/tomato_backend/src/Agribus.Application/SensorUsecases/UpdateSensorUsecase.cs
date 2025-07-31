using Agribus.Core.Ports.Api.SensorUsecases;
using Agribus.Core.Ports.Api.SensorUsecases.DTOs;
using Agribus.Core.Ports.Spi.SensorContext;

namespace Agribus.Application.SensorUsecases;

public class UpdateSensorUsecase(ISensorRepository sensorRepository) : IUpdateSensorUsecase
{
    public async Task<bool?> Handle(
        Guid originalSensorId,
        Guid userId,
        UpdateSensorDto dto,
        CancellationToken cancellationToken
    )
    {
        var sensor = await sensorRepository.Exists(originalSensorId, userId, cancellationToken);
        if (sensor == null)
            return false;

        return await sensorRepository.UpdateAsync(sensor, dto, cancellationToken);
    }
}
