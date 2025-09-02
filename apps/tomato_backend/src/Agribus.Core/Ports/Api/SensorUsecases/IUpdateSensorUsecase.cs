using Agribus.Core.Ports.Api.SensorUsecases.DTOs;

namespace Agribus.Core.Ports.Api.SensorUsecases;

public interface IUpdateSensorUsecase
{
    Task<bool?> Handle(
        Guid originalSensorId,
        string userId,
        UpdateSensorDto dto,
        CancellationToken cancellationToken
    );
    Task<bool?> HandleFromGreenhouse(
        string userId,
        UpdateSensorFromGreenhouseDto dto,
        CancellationToken cancellationToken
    );
}
