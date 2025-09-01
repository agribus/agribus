using Agribus.Core.Ports.Api.GenericUsecases;
using Agribus.OpenMeteo.Services;
using NSubstitute;

namespace Agribus.UnitTests.MeteoServices;

public class GeocodingApiServiceTests
{
    [Fact]
    public async Task ShouldGetCoordinates_GivenValidCityAndCountry()
    {
        // Given
        var httpService = Substitute.For<IGetHttpUsecase>();
        var expectedJsonResponse = """
            {
              "results": [
                {
                  "id": 2950159,
                  "name": "Berlin",
                  "latitude": 52.52437,
                  "longitude": 13.41053,
                  "elevation": 74,
                  "feature_code": "PPLC",
                  "country_code": "DE",
                  "admin1_id": 2950157,
                  "admin3_id": 6547383,
                  "admin4_id": 6547539,
                  "timezone": "Europe/Berlin",
                  "population": 3426354,
                  "postcodes": [
                    "10967",
                    "13347"
                  ],
                  "country_id": 2921044,
                  "country": "Germany",
                  "admin1": "State of Berlin",
                  "admin3": "Berlin, Stadt",
                  "admin4": "Berlin"
                }
              ],
              "generationtime_ms": 0.44429302
            }
            """;

        httpService
            .GetAsync(
                "https://geocoding-api.open-meteo.com/v1/search",
                Arg.Any<Dictionary<string, string>>()
            )
            .Returns(expectedJsonResponse);

        var service = new GeocodingApiService(httpService);
        var city = "Berlin";
        var country = "Germany";

        // When
        var result = await service.GetCoordinatesAsync(city, country);

        // Then
        Assert.Equal("52.52437", result.Item1);
        Assert.Equal("13.41053", result.Item2);

        await httpService
            .Received(1)
            .GetAsync(
                "https://geocoding-api.open-meteo.com/v1/search",
                Arg.Is<Dictionary<string, string>>(d =>
                    d["name"] == city && d["country"] == country && d["count"] == "1"
                )
            );
    }

    [Fact]
    public async Task ShouldThrowException_WhenNoResultsFound()
    {
        // Given
        var httpService = Substitute.For<IGetHttpUsecase>();
        var emptyJsonResponse = """
            {
              "generationtime_ms": 0.44429302
            }
            """;

        httpService
            .GetAsync(Arg.Any<string>(), Arg.Any<Dictionary<string, string>>())
            .Returns(emptyJsonResponse);

        var service = new GeocodingApiService(httpService);

        // When & Then
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.GetCoordinatesAsync("UnknownCity", "UnknownCountry")
        );
    }
}
