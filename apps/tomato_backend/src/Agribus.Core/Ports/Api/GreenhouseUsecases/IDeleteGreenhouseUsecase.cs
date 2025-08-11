namespace Agribus.Core.Ports.Api.GreenhouseUsecases;

public interface IDeleteGreenhouseUsecase
{
    Task<bool> Handle(Guid greenhouseId, string userId, CancellationToken cancellationToken);
}
