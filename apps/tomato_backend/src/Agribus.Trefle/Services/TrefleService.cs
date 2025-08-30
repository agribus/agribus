using System.Text.Json;
using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
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
        var cropId = await GetCropSpeciesId(scientificName);

        return await GetSpeciesGrowthConditions(cropId);
    }

    private async Task<int> GetCropSpeciesId(string scientificName)
    {
        var searchParameters = new Dictionary<string, string>(_parameters)
        {
            ["q"] = scientificName,
        };

        var jsonResponse = await _getHttpUsecase.GetAsync(
            $"{_baseUrl}/species/search",
            searchParameters
        );
        var jsonElement = JsonDocument.Parse(jsonResponse).RootElement;
        if (!jsonElement.TryGetProperty("data", out var dataElement))
        {
            throw new InvalidOperationException(
                $"No species found for scientific name: {scientificName}"
            );
        }
        return jsonElement.GetProperty("data").GetProperty("id").GetInt32();
    }

    private async Task<CropGrowthConditions> GetSpeciesGrowthConditions(int cropId)
    {
        var jsonResponse = await _getHttpUsecase.GetAsync(
            $"{_baseUrl}/species/{cropId}",
            _parameters
        );
        var jsonElement = JsonDocument.Parse(jsonResponse).RootElement;

        return new CropGrowthConditions
        {
            AtmosphericHumidity = jsonElement
                .GetProperty("data")
                .GetProperty("growth")
                .GetProperty("atmospheric_humidity")
                .GetSingle(),
            MinimumTemperature = jsonElement
                .GetProperty("data")
                .GetProperty("growth")
                .GetProperty("minimum_temperature")
                .GetProperty("deg_c")
                .GetSingle(),
            MaximumTemperature = jsonElement
                .GetProperty("data")
                .GetProperty("growth")
                .GetProperty("maximum_temperature")
                .GetProperty("deg_c")
                .GetSingle(),
        };
    }
}
