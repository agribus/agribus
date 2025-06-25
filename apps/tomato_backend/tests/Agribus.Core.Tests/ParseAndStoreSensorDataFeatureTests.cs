using Agribus.Core.Features;
using Agribus.Core.Ports.Api.DTOs;
using Agribus.Core.Ports.Api.Interfaces;
using Agribus.Core.Ports.Api.Validators;
using FluentValidation;

namespace Agribus.Core.Tests;

public class ParseAndStoreSensorDataFeatureTests
{
    private readonly IParseSensorData _feature;

    public ParseAndStoreSensorDataFeatureTests()
    {
        var validator = new RawSensorPayloadValidator();
        _feature = new ParseAndStoreSensorDataFeature(validator);
    }

    [Theory]
    [InlineData("Temperature")] 
    [InlineData("Humidity")] 
    [InlineData("Pressure")] 
    public async Task GivenCorrectSensorData_ShouldSuccessfullyParse(string type)
    {
        // Given
        var data = BuildPayload(type);

        // When
        var result = await _feature.FromRawJson(data, CancellationToken.None);

        // Should
        Assert.True(result);
    }

    [Fact]
    public async Task GivenInvalidSensorData_ShouldFail()
    {
        // Given
        var data = BuildPayload("X");
        
        // When
        var result = async () => await _feature.FromRawJson(data, CancellationToken.None);
        
        // Should
        await Assert.ThrowsAsync<ValidationException>(result);
    }
    

    private RawSensorPayload BuildPayload(string? type = null)
    {
        return new RawSensorPayload
        {
            Type = type ?? "Temperature",
            SourceAdress = "ruuvi-114",
            Timestamp = "1750743684750",
            Value = 13.0,
        };
    }
}
