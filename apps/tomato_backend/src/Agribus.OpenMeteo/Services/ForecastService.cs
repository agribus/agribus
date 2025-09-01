using System.Text.Json;
using Agribus.Core.Domain.AggregatesModels.OpenMeteoAggregates;
using Agribus.Core.Ports.Api.GenericUsecases;
using Agribus.Core.Ports.Spi.OpenMeteoContext;

namespace Agribus.OpenMeteo.Services;

public class ForecastService : IForecastService
{
    private readonly IGetHttpUsecase _getHttpUsecase;
    private readonly string _baseUrl = "https://api.open-meteo.com/v1/forecast";
    private Dictionary<string, string> _parameters = [];

    public ForecastService(IGetHttpUsecase getHttpUsecase)
    {
        _getHttpUsecase = getHttpUsecase;
        _parameters.Add(
            "hourly",
            "temperature_2m,relative_humidity_2m,weather_code,pressure_msl,precipitation_probability,precipitation"
        );
    }

    public async Task<List<ForecastHourly>> GetForecastAsync(string lat, string lon)
    {
        _parameters.Add("latitude", lat);
        _parameters.Add("longitude", lon);
        var jsonResponse = await _getHttpUsecase.GetAsync(_baseUrl, _parameters);
        return ParseOpenMeteoResponse(jsonResponse);
    }

    private List<ForecastHourly> ParseOpenMeteoResponse(string jsonResponse)
    {
        var jsonDocument = JsonDocument.Parse(jsonResponse);
        var jsonElement = jsonDocument.RootElement;

        if (!jsonElement.TryGetProperty("hourly", out var hourlyElement))
        {
            throw new InvalidOperationException("No hourly data found in the response");
        }

        var times = hourlyElement
            .GetProperty("time")
            .EnumerateArray()
            .Select(x => DateTime.Parse(x.GetString()!))
            .ToArray();

        var temperatures = hourlyElement
            .GetProperty("temperature_2m")
            .EnumerateArray()
            .Select(x => (float?)x.GetSingle())
            .ToArray();

        var humidities = hourlyElement
            .GetProperty("relative_humidity_2m")
            .EnumerateArray()
            .Select(x => (int?)x.GetInt32())
            .ToArray();

        var weatherCodes = hourlyElement
            .GetProperty("weather_code")
            .EnumerateArray()
            .Select(x => (WeatherCode?)x.GetInt32())
            .ToArray();

        var pressures = hourlyElement
            .GetProperty("pressure_msl")
            .EnumerateArray()
            .Select(x => (float?)x.GetSingle())
            .ToArray();

        var precipitationProbabilities = hourlyElement
            .GetProperty("precipitation_probability")
            .EnumerateArray()
            .Select(x => (int?)x.GetInt32())
            .ToArray();

        var precipitations = hourlyElement
            .GetProperty("precipitation")
            .EnumerateArray()
            .Select(x => (float?)x.GetSingle())
            .ToArray();

        return times
            .Select(
                (t, i) =>
                    new ForecastHourly
                    {
                        Time = t,
                        Temperature = i < temperatures.Length ? temperatures[i] : null,
                        Humidity = i < humidities.Length ? humidities[i] : null,
                        WeatherCode = i < weatherCodes.Length ? weatherCodes[i] : null,
                        Pressure = i < pressures.Length ? pressures[i] : null,
                        PrecipitationProbability =
                            i < precipitationProbabilities.Length
                                ? precipitationProbabilities[i]
                                : null,
                        Precipitation = i < precipitations.Length ? precipitations[i] : null,
                    }
            )
            .ToList();
    }
}
