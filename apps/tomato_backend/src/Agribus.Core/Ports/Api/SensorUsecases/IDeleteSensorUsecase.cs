namespace Agribus.Core.Ports.Api.SensorUsecases;

public interface IDeleteSensorUsecase
{
    Task<bool> Handle(Guid sensorId, string userId, CancellationToken cancellationToken);
}
