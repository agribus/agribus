using Agribus.Core.Ports.Api.SensorUsecases;
using Agribus.Core.Ports.Spi.SensorContext;

namespace Agribus.Application.SensorUsecases;

public class DeleteSensorUsecase(ISensorRepository sensorRepository) : IDeleteSensorUsecase
{
    public async Task<bool> Handle(
        Guid sensorId,
        string userId,
        CancellationToken cancellationToken
    )
    {
        var sensor = await sensorRepository.Exists(sensorId, userId, cancellationToken);
        if (sensor is null)
            return false;

        return await sensorRepository.DeleteAsync(sensor, cancellationToken);
    }
}
