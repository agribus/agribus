using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Ports.Api.GreenhouseUsecases;
using Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;
using Agribus.Core.Ports.Spi.AuthContext;
using Agribus.Core.Ports.Spi.GreenhouseContext;

namespace Agribus.Application.GreenhouseUsecases;

public class CreateGreenhouseUsecase(IGreenhouseRepository greenhouseRepository)
    : ICreateGreenhouseUsecase
{
    public async Task<Greenhouse> Handle(
        CreateGreenhouseDto dto,
        string userId,
        CancellationToken cancellationToken
    )
    {
        var entityToAdd = dto.MapToGreenhouse(userId);
        var result = await greenhouseRepository.AddAsync(entityToAdd, cancellationToken);
        return result;
    }
}
