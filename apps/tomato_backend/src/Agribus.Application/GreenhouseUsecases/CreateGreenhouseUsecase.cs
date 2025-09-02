using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Ports.Api.GreenhouseUsecases;
using Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;
using Agribus.Core.Ports.Spi.GreenhouseContext;
using Agribus.Core.Ports.Spi.OpenMeteoContext;
using Agribus.Core.Ports.Spi.TrefleContext;

namespace Agribus.Application.GreenhouseUsecases;

public class CreateGreenhouseUsecase(
    IGreenhouseRepository greenhouseRepository,
    IGeocodingApiService geocodingApiService,
    ITrefleService trefleService
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
        // foreach (var crop in entityToAdd.Crops)
        // {
        //     var cropGrowthConditions = await trefleService.GetCropIdealConditions(
        //         crop.ScientificName
        //     );
        //     crop.AddCropGrowthConditions(cropGrowthConditions);
        // }
        var result = await greenhouseRepository.AddAsync(entityToAdd, cancellationToken);
        return result;
    }
}
