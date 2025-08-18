using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Ports.Api.GreenhouseUsecases;
using Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;
using Agribus.Core.Ports.Spi.GreenhouseContext;
using Agribus.Core.Ports.Spi.OpenMeteoContext;

namespace Agribus.Application.GreenhouseUsecases;

public class UpdateGreenhouseUsecase(
    IGreenhouseRepository greenhouseRepository,
    IGeocodingApiService geocodingApiService
) : IUpdateGreenhouseUsecase
{
    public async Task<bool?> Handle(
        Guid greenhouseId,
        string userId,
        UpdateGreenhouseDto dto,
        CancellationToken cancellationToken
    )
    {
        var greenhouse = await greenhouseRepository.Exists(greenhouseId, userId, cancellationToken);
        if (greenhouse is null)
            return false;
        var (lat, lon) = await geocodingApiService.GetCoordinatesAsync(dto.City, dto.Country);
        greenhouse.AddCoordinate(lat, lon);
        return await greenhouseRepository.UpdateAsync(greenhouse, dto, cancellationToken);
    }
}
