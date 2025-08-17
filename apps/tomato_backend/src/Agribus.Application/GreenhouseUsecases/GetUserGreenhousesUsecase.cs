using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Ports.Api.GreenhouseUsecases;
using Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;
using Agribus.Core.Ports.Spi.GreenhouseContext;

namespace Agribus.Application.GreenhouseUsecases;

public class GetUserGreenhousesUsecase(IGreenhouseRepository greenhouseRepository)
    : IGetUserGreenhousesUsecase
{
    public async Task<IEnumerable<GreenhouseListItemDto>> Handle(
        string userId,
        CancellationToken cancellationToken
    )
    {
        return await greenhouseRepository.GetByUserIdAsync(userId, cancellationToken);
    }
}
