namespace Agribus.Core.Ports.Api.AlertUsecases;

public interface IDeleteAlertUsecase
{
    Task<bool> Handle(Guid alertId, string userId, CancellationToken cancellationToken);
}
