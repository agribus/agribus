using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Ports.Api.GreenhouseUsecases;
using Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;
using Agribus.Core.Ports.Spi.AuthContext;
using Agribus.Core.Ports.Spi.GreenhouseContext;

namespace Agribus.Application.GreenhouseUsecases;

public class CreateGreenhouseUsecase(
    IAuthContextService authContext,
    IGreenhouseRepository greenhouseRepository
) : ICreateGreenhouseUsecase
{
    public Task<Greenhouse> Handle(
        CreateGreenhouseDto dto,
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        var entityToAdd = dto.MapToGreenhouse();
        var result = greenhouseRepository.AddAsync(entityToAdd, userId, cancellationToken);
        return result;
    }
}
