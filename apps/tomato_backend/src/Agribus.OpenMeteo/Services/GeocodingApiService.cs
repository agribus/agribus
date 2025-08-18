using System.Text.Json;
using Agribus.Core.Ports.Spi.OpenMeteoContext;

namespace Agribus.OpenMeteo.Services;

public class GeocodingApiService : IGeocodingApiService
{
    private readonly IHttpService _httpService;
    private readonly string _baseUrl = "https://geocoding-api.open-meteo.com/v1/search";
    private Dictionary<string, string> _parameters = [];

    public GeocodingApiService(IHttpService httpService)
    {
        _httpService = httpService;
        _parameters.Add("count", "1");
    }

    public async Task<(string, string)> GetCoordinatesAsync(string city, string country)
    {
        _parameters.Add("name", city);
        _parameters.Add("country", country);
        var jsonResponse = await _httpService.GetAsync(_baseUrl, _parameters);

        var jsonDocument = JsonDocument.Parse(jsonResponse);
        var jsonElement = jsonDocument.RootElement;
        if (!jsonElement.TryGetProperty("results", out var resultsElement))
        {
            throw new InvalidOperationException("No result data found in the response");
        }

        var lat = resultsElement[0].GetProperty("latitude").GetRawText();
        var lon = resultsElement[0].GetProperty("longitude").GetRawText();

        return (lat, lon);
    }
}
