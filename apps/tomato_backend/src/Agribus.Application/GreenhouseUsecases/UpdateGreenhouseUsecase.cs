using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Ports.Api.GreenhouseUsecases;
using Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;
using Agribus.Core.Ports.Spi.GreenhouseContext;

namespace Agribus.Application.GreenhouseUsecases;

public class UpdateGreenhouseUsecase(
    // IAuthContextService authContext,
    IGreenhouseRepository greenhouseRepository
) : IUpdateGreenhouseUsecase
{
    public async Task<bool?> Handle(
        Guid greenhouseId,
        Guid userId,
        UpdateGreenhouseDto dto,
        CancellationToken cancellationToken
    )
    {
        var greenhouse = await greenhouseRepository.Exists(greenhouseId, userId, cancellationToken);
        if (greenhouse == null)
            return false;

        greenhouse.Update(dto);

        return await greenhouseRepository.UpdateAsync(greenhouse, cancellationToken);
    }
}
