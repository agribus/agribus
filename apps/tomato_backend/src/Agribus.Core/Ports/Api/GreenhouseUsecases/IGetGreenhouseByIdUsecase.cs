using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;

namespace Agribus.Application.GreenhouseUsecases;

public interface IGetGreenhouseByIdUsecase
{
    Task<Greenhouse?> Handle(Guid greenhouseId, string userId, CancellationToken cancellationToken);
}
