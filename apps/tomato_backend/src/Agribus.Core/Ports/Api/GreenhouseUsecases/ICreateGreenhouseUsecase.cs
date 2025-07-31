using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;

namespace Agribus.Core.Ports.Api.GreenhouseUsecases;

public interface ICreateGreenhouseUsecase
{
    Task<Greenhouse> Handle(
        CreateGreenhouseDto dto,
        Guid userId,
        CancellationToken cancellationToken
    );
}
