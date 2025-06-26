namespace Agribus.Core.Ports.Api.DTOs;

public record RawSensorPayload
{
    public required string Type { get; init; }
    public required string SourceAdress { get; init; }
    public required long Timestamp { get; init; }
    public required float Value { get; init; }
}
