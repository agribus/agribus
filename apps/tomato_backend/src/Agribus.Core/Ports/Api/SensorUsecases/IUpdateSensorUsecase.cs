using Agribus.Core.Ports.Api.SensorUsecases.DTOs;

namespace Agribus.Core.Ports.Api.SensorUsecases;

public interface IUpdateSensorUsecase
{
    Task<bool?> Handle(
        Guid originalSensorId,
        Guid userId,
        UpdateSensorDto dto,
        CancellationToken cancellationToken
    );
}
