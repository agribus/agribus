using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Ports.Api.GreenhouseUsecases;
using Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;
using Agribus.Core.Ports.Api.SensorUsecases;
using Agribus.Core.Ports.Api.SensorUsecases.DTOs;
using Agribus.Core.Ports.Spi.GreenhouseContext;
using Agribus.Core.Ports.Spi.SensorContext;
using Agribus.Core.Ports.Spi.OpenMeteoContext;
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
        foreach (var crop in greenhouse.Crops)
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
        var currentSensors = await sensorRepository.GetByGreenhouseIdAsync(
            greenhouse.Id,
            userId,
            cancellationToken
        );
        if (dtoSensors is null || dtoSensors.Count == 0)
        {
            foreach (var sensor in currentSensors)
            {
                await sensorRepository.DeleteAsync(sensor, cancellationToken);
            }

            return;
        }

        var requestIds = dtoSensors.Where(s => s.Id != Guid.Empty).ToDictionary(s => s.Id, s => s);
        foreach (var sensor in currentSensors)
        {
            if (!requestIds.ContainsKey(sensor.Id))
            {
                await sensorRepository.DeleteAsync(sensor, cancellationToken);
            }
        }

        foreach (var dto in requestIds.Values)
        {
            await updateSensorUsecase.HandleFromGreenhouse(
                greenhouse.UserId,
                dto,
                cancellationToken
            );
        }
    }
}
