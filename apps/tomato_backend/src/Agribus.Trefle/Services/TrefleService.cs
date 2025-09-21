using System.Text.Json;
using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Domain.Exceptions;
using Agribus.Core.Ports.Api.GenericUsecases;
using Agribus.Core.Ports.Spi.TrefleContext;
using Microsoft.Extensions.Configuration;

namespace Agribus.Trefle.Services;

public class TrefleService : ITrefleService
{
    private readonly IGetHttpUsecase _getHttpUsecase;
    private readonly IConfiguration _configuration;
    private readonly string _baseUrl = "https://trefle.io/api/v1";
    private Dictionary<string, string> _parameters = [];

    public TrefleService(IGetHttpUsecase getHttpUsecase, IConfiguration configuration)
    {
        _getHttpUsecase = getHttpUsecase;
        _configuration = configuration;
        _parameters.Add("token", _configuration["Trefle:Token"]);
    }

    public async Task<CropGrowthConditions> GetCropIdealConditions(string scientificName)
    {
        try
        {
            var cropId = await GetCropSpeciesId(scientificName);
            return await GetSpeciesGrowthConditions(cropId);
        }
        catch (NotFoundEntityException)
        {
            return new CropGrowthConditions
            {
                AtmosphericHumidity = null,
                MinimumTemperature = null,
                MaximumTemperature = null,
            };
        }
    }

    private async Task<int> GetCropSpeciesId(string scientificName)
    {
        var searchParameters = new Dictionary<string, string>(_parameters)
        {
            ["filter[scientific_name]"] = scientificName,
        };

        var jsonResponse = await _getHttpUsecase.GetAsync($"{_baseUrl}/species", searchParameters);
        var jsonElement = JsonDocument.Parse(jsonResponse).RootElement;
        jsonElement.TryGetProperty("data", out var dataElement);
        if (dataElement.GetArrayLength() == 0)
            throw new NotFoundEntityException(
                $"No species found for scientific name: {scientificName}"
            );
        dataElement[0].TryGetProperty("scientific_name", out var responseScientificName);
        return responseScientificName.GetString().ToLower() != scientificName
            ? throw new NotFoundEntityException(
                $"No species found for scientific name: {scientificName}"
            )
            : dataElement[0].GetProperty("id").GetInt32();
    }

    private async Task<CropGrowthConditions> GetSpeciesGrowthConditions(int cropId)
    {
        var jsonResponse = await _getHttpUsecase.GetAsync(
            $"{_baseUrl}/species/{cropId}",
            _parameters
        );
        var jsonElement = JsonDocument.Parse(jsonResponse).RootElement;

        var dataElement = jsonElement.GetProperty("data");
        var growthElement = dataElement.GetProperty("growth");

        return new CropGrowthConditions
        {
            AtmosphericHumidity = GetNullableFloat(growthElement, "atmospheric_humidity"),
            MinimumTemperature = GetNullableTemperature(growthElement, "minimum_temperature"),
            MaximumTemperature = GetNullableTemperature(growthElement, "maximum_temperature"),
        };
    }

    private float? GetNullableFloat(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
            return null;
        return property.ValueKind switch
        {
            JsonValueKind.Number => property.GetSingle(),
            _ => null,
        };
    }

    private float? GetNullableTemperature(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var tempProperty))
            return null;
        if (
            tempProperty.ValueKind == JsonValueKind.Object
            && tempProperty.TryGetProperty("deg_c", out var degCProperty)
        )
        {
            switch (degCProperty.ValueKind)
            {
                case JsonValueKind.Number:
                    return degCProperty.GetSingle();
                case JsonValueKind.Null:
                    return null;
            }
        }

        return null;
    }
}
