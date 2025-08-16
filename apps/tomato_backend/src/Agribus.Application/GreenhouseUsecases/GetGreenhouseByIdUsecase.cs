using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Ports.Spi.GreenhouseContext;

namespace Agribus.Application.GreenhouseUsecases;

public class GetGreenhouseByIdUsecase(IGreenhouseRepository greenhouseRepository)
    : IGetGreenhouseByIdUsecase
{
    public async Task<Greenhouse?> Handle(
        Guid greenhouseId,
        string userId,
        CancellationToken cancellationToken
    )
    {
        return await greenhouseRepository.GetByIdAsync(greenhouseId, userId, cancellationToken);
    }
}
