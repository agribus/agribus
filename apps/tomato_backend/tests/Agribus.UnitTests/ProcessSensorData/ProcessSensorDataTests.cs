using Agribus.Core.Enums;
using Agribus.Core.Ports.Api.DTOs;
using Agribus.Core.Ports.Api.ParseSensorData;
using Agribus.Core.Ports.Api.Validators;
using FluentValidation;

namespace Agribus.UnitTests.ProcessSensorData;

public class ProcessSensorDataTests
{
    private readonly IParseSensorData _parser;

    public ProcessSensorDataTests()
    {
        _parser = new ParseSensorData(new RawSensorPayloadValidator());
    }

    [Theory]
    [InlineData("temperature", 22.5f, SensorType.Temperature)]
    [InlineData("humidity", 60.1f, SensorType.Humidity)]
    [InlineData("pressure", 1013.25f, SensorType.Pressure)]
    [InlineData("motion", 1.0f, SensorType.Motion)]
    [InlineData("neighbors", 3.0f, SensorType.Neighbors)]
    public async Task Should_ParsePayload_Correctly(string type, float value, SensorType expected)
    {
        // Given
        var payload = new RawSensorPayload
        {
            Type = type,
            SourceAdress = "abc123",
            Timestamp = 1750743600000,
            Value = value,
        };

        // When
        var result = await _parser.FromRawJson(payload);

        // Then
        Assert.Equal(expected, result.Type);
        Assert.Equal(value, result.Value);
        Assert.Equal("abc123", result.SourceAdress);
        Assert.Equal(
            DateTimeOffset.FromUnixTimeMilliseconds(payload.Timestamp).UtcDateTime,
            result.Date
        );
    }

    // TODO UNKNOWN TYPE SHOULD THROW
    [Fact]
    public async Task Should_ThrowValidationException_GivenInvalidPayload()
    {
        // Given
        var payload = new RawSensorPayload
        {
            Type = "invalid_type",
            SourceAdress = "",
            Timestamp = 1750743600000,
            Value = 10.0f,
        };

        // When
        var action = async () => await _parser.FromRawJson(payload);

        // Then
        await Assert.ThrowsAsync<ValidationException>(action);
    }
}
