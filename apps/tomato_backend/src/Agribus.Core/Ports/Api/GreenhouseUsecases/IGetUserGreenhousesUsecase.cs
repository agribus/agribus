using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;

namespace Agribus.Core.Ports.Api.GreenhouseUsecases;

public interface IGetUserGreenhousesUsecase
{
    Task<IEnumerable<GreenhouseListItemDto>> Handle(
        string userId,
        CancellationToken cancellationToken
    );
}
