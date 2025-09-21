using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Ports.Api.GreenhouseUsecases;
using Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;
using Agribus.Core.Ports.Api.SensorUsecases;
using Agribus.Core.Ports.Api.SensorUsecases.DTOs;
using Agribus.Core.Ports.Spi.GreenhouseContext;
using Agribus.Core.Ports.Spi.OpenMeteoContext;
using Agribus.Core.Ports.Spi.SensorContext;
using Agribus.Core.Ports.Spi.TrefleContext;

namespace Agribus.Application.GreenhouseUsecases;

public class UpdateGreenhouseUsecase(
    IGreenhouseRepository greenhouseRepository,
    IUpdateSensorUsecase updateSensorUsecase,
    ISensorRepository sensorRepository,
    IGeocodingApiService geocodingApiService,
    ITrefleService trefleService
) : IUpdateGreenhouseUsecase
{
    public async Task<bool> Handle(
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
        foreach (var crop in dto.Crops)
        {
            var cropGrowthConditions = await trefleService.GetCropIdealConditions(
                crop.ScientificName
            );
            crop.AddCropGrowthConditions(cropGrowthConditions);
        }
        await greenhouseRepository.UpdateAsync(greenhouse, dto, cancellationToken);

        await SyncSensorsAsync(greenhouse, dto.Sensors, userId, cancellationToken);

        return true;
    }

    private async Task SyncSensorsAsync(
        Greenhouse greenhouse,
        List<UpdateSensorFromGreenhouseDto>? dtoSensors,
        string userId,
        CancellationToken cancellationToken
    )
    {
        if (dtoSensors is null)
            return;

        var currentSensors = await sensorRepository.GetByGreenhouseIdAsync(
            greenhouse.Id,
            userId,
            cancellationToken
        );

        if (dtoSensors.Count == 0)
        {
            foreach (var sensor in currentSensors)
            {
                await sensorRepository.DeleteAsync(sensor, cancellationToken);
            }

            return;
        }

        var currentBySource = currentSensors.ToDictionary(s => s.SourceAddress, s => s);
        var requestBySource = dtoSensors
            .Where(d => !string.IsNullOrWhiteSpace(d.SourceAddress))
            .ToDictionary(d => d.SourceAddress, d => d);

        foreach (var dto in requestBySource.Values)
        {
            if (currentBySource.TryGetValue(dto.SourceAddress, out var current))
            {
                dto.Id = current.Id;
                await updateSensorUsecase.HandleFromGreenhouse(
                    greenhouse.UserId,
                    dto,
                    cancellationToken
                );
            }
        }
    }
}
