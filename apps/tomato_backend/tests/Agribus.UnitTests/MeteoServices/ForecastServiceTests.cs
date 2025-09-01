using Agribus.Core.Domain.AggregatesModels.OpenMeteoAggregates;
using Agribus.Core.Ports.Api.GenericUsecases;
using Agribus.OpenMeteo.Services;
using NSubstitute;

namespace Agribus.UnitTests.MeteoServices;

public class ForecastServiceTests
{
    [Fact]
    public async Task ShouldGetForecast_GivenValidCoordinates()
    {
        // Given
        var httpService = Substitute.For<IGetHttpUsecase>();
        var expectedJsonResponse = """
            {
              "latitude": 52.52,
              "longitude": 13.419998,
              "generationtime_ms": 0.178217887878418,
              "utc_offset_seconds": 0,
              "timezone": "GMT",
              "timezone_abbreviation": "GMT",
              "elevation": 38,
              "hourly_units": {
                "time": "iso8601",
                "temperature_2m": "°C",
                "relative_humidity_2m": "%",
                "weather_code": "wmo code",
                "pressure_msl": "hPa",
                "precipitation_probability": "%",
                "precipitation": "mm"
              },
              "hourly": {
                "time": [
                  "2025-08-12T00:00",
                  "2025-08-12T01:00",
                  "2025-08-12T02:00"
                ],
                "temperature_2m": [19.0, 18.3, 17.5],
                "relative_humidity_2m": [68, 69, 71],
                "weather_code": [0, 0, 1],
                "pressure_msl": [1022.4, 1022.1, 1021.9],
                "precipitation_probability": [0, 0, 0],
                "precipitation": [0.0, 0.0, 0.0]
              }
            }
            """;

        httpService
            .GetAsync(
                "https://api.open-meteo.com/v1/forecast",
                Arg.Any<Dictionary<string, string>>()
            )
            .Returns(expectedJsonResponse);

        var service = new ForecastService(httpService);
        var latitude = "52.52";
        var longitude = "13.41";

        // When
        var result = await service.GetForecastAsync(latitude, longitude);

        // Then
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);

        var firstHour = result[0];
        Assert.Equal(DateTime.Parse("2025-08-12T00:00"), firstHour.Time);
        Assert.Equal(19.0f, firstHour.Temperature);
        Assert.Equal(68, firstHour.Humidity);
        Assert.Equal(WeatherCode.ClearSky, firstHour.WeatherCode);
        Assert.Equal(1022.4f, firstHour.Pressure);
        Assert.Equal(0, firstHour.PrecipitationProbability);
        Assert.Equal(0.0f, firstHour.Precipitation);

        var secondHour = result[1];
        Assert.Equal(DateTime.Parse("2025-08-12T01:00"), secondHour.Time);
        Assert.Equal(18.3f, secondHour.Temperature);
        Assert.Equal(69, secondHour.Humidity);
        Assert.Equal(WeatherCode.ClearSky, secondHour.WeatherCode);

        var thirdHour = result[2];
        Assert.Equal(DateTime.Parse("2025-08-12T02:00"), thirdHour.Time);
        Assert.Equal(17.5f, thirdHour.Temperature);
        Assert.Equal(71, thirdHour.Humidity);
        Assert.Equal(WeatherCode.MainlyClear, thirdHour.WeatherCode);

        await httpService
            .Received(1)
            .GetAsync(
                "https://api.open-meteo.com/v1/forecast",
                Arg.Is<Dictionary<string, string>>(d =>
                    d["latitude"] == latitude
                    && d["longitude"] == longitude
                    && d["hourly"]
                        == "temperature_2m,relative_humidity_2m,weather_code,pressure_msl,precipitation_probability,precipitation"
                )
            );
    }

    [Fact]
    public async Task ShouldThrowException_WhenNoHourlyDataFound()
    {
        // Given
        var httpService = Substitute.For<IGetHttpUsecase>();
        var invalidJsonResponse = """
            {
              "latitude": 52.52,
              "longitude": 13.419998,
              "generationtime_ms": 0.178217887878418
            }
            """;

        httpService
            .GetAsync(Arg.Any<string>(), Arg.Any<Dictionary<string, string>>())
            .Returns(invalidJsonResponse);

        var service = new ForecastService(httpService);

        // When & Then
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.GetForecastAsync("52.52", "13.41")
        );
    }

    [Fact]
    public async Task ShouldHandleMismatchedArrayLengths_InMeteoData()
    {
        // Given
        var httpService = Substitute.For<IGetHttpUsecase>();
        var jsonWithMismatchedArrays = """
            {
              "hourly": {
                "time": [
                  "2025-08-12T00:00",
                  "2025-08-12T01:00",
                  "2025-08-12T02:00"
                ],
                "temperature_2m": [19.0, 18.3],
                "relative_humidity_2m": [68],
                "weather_code": [0, 0, 1],
                "pressure_msl": [1022.4, 1022.1, 1021.9],
                "precipitation_probability": [0, 0],
                "precipitation": [0.0]
              }
            }
            """;

        httpService
            .GetAsync(Arg.Any<string>(), Arg.Any<Dictionary<string, string>>())
            .Returns(jsonWithMismatchedArrays);

        var service = new ForecastService(httpService);

        // When
        var result = await service.GetForecastAsync("52.52", "13.41");

        // Then
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);

        // All data is complete
        var firstHour = result[0];
        Assert.Equal(DateTime.Parse("2025-08-12T00:00"), firstHour.Time);
        Assert.Equal(19.0f, firstHour.Temperature);
        Assert.Equal(68, firstHour.Humidity);
        Assert.Equal(WeatherCode.ClearSky, firstHour.WeatherCode);
        Assert.Equal(1022.4f, firstHour.Pressure);
        Assert.Equal(0, firstHour.PrecipitationProbability);
        Assert.Equal(0.0f, firstHour.Precipitation);

        // Some data missing
        var secondHour = result[1];
        Assert.Equal(DateTime.Parse("2025-08-12T01:00"), secondHour.Time);
        Assert.Equal(18.3f, secondHour.Temperature);
        Assert.Null(secondHour.Humidity); // Array trop court
        Assert.Equal(WeatherCode.ClearSky, secondHour.WeatherCode);
        Assert.Equal(1022.1f, secondHour.Pressure);
        Assert.Equal(0, secondHour.PrecipitationProbability);
        Assert.Null(secondHour.Precipitation); // Array trop court

        // More data missing
        var thirdHour = result[2];
        Assert.Equal(DateTime.Parse("2025-08-12T02:00"), thirdHour.Time);
        Assert.Null(thirdHour.Temperature); // Array trop court
        Assert.Null(thirdHour.Humidity); // Array trop court
        Assert.Equal(WeatherCode.MainlyClear, thirdHour.WeatherCode);
        Assert.Equal(1021.9f, thirdHour.Pressure);
        Assert.Null(thirdHour.PrecipitationProbability); // Array trop court
        Assert.Null(thirdHour.Precipitation); // Array trop court
    }
}
