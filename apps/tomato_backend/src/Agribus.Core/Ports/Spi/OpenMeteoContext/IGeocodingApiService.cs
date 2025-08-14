namespace Agribus.Core.Ports.Spi.OpenMeteoContext;

public interface IGeocodingApiService
{
    Task<(float, float)> GetCoordinatesAsync(string city, string country);
}
