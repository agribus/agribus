using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Ports.Api.GreenhouseUsecases;
using Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;
using Agribus.Core.Ports.Spi.GreenhouseContext;
using Agribus.Core.Ports.Spi.OpenMeteoContext;

namespace Agribus.Application.GreenhouseUsecases;

public class CreateGreenhouseUsecase(
    IGreenhouseRepository greenhouseRepository,
    IGeocodingApiService geocodingApiService
) : ICreateGreenhouseUsecase
{
    public async Task<Greenhouse> Handle(
        CreateGreenhouseDto dto,
        string userId,
        CancellationToken cancellationToken
    )
    {
        var entityToAdd = dto.MapToGreenhouse(userId);
        var (lat, lon) = await geocodingApiService.GetCoordinatesAsync(dto.City, dto.Country);
        entityToAdd.AddCoordinate(lat, lon);
        var result = await greenhouseRepository.AddAsync(entityToAdd, cancellationToken);
        return result;
    }
}
