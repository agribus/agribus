using Agribus.Core.Enums;

namespace Agribus.Core.Ports.Api.DTOs;

public record RawSensorPayload
{
    public string Type { get; init; }
    public string SensorId { get; init; }
    public string Timestamp { get; init; }
    public double Value { get; init; }
}
