namespace Agribus.Core.Ports.Api.ParseSensorData.DTOs;

public record RawSensorPayload
{
    public required string Type { get; init; }
    public required string SourceAddress { get; init; }
    public required long Timestamp { get; init; }
    public required float Value { get; init; }
}
