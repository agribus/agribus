namespace Agribus.Core.Ports.Spi.OpenMeteoContext;

public interface IGeocodingApiService
{
    Task<(string, string)> GetCoordinatesAsync(string city, string country);
}
