namespace Agribus.Core.Ports.Api.GreenhouseUsecases;

public interface IDeleteGreenhouseUsecase
{
    Task<bool> Handle(Guid greenhouseId, Guid userId, CancellationToken cancellationToken);
}
