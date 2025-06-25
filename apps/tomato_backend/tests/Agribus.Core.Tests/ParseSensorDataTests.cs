using Agribus.Core.Enums;
using Agribus.Core.Ports.Api.DTOs;

namespace Agribus.Core.Tests;

public class ParseSensorDataTests
{
    private readonly IParseSensorData _feature;

    public ParseSensorDataTests()
    {
        _feature = new ParseSensorData();
    }

    [Fact]
    public async Task GivenCorrectSensorData_ShouldSuccessfullyParse()
    {
        // Given
        var data = BuildPayload();

        // When
        var result = await _feature.FromRawJson(data, CancellationToken.None);

        // Should
        Assert.Equal(result, true);
        // should call influxdb store
    }

    private RawSensorPayload BuildPayload(SensorType? type = null)
    {
        return new RawSensorPayload
        {
            Type = type ?? SensorType.Temperature,
            SensorId = "114",
            Timestamp = "1750743684750",
            Value = 13,
        };
    }
}
