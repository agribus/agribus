namespace Agribus.Core.Ports.Api.DTOs;

public record RawSensorPayload
{
    public string Type { get; init; }
    public string SourceAdress { get; init; }
    public string Timestamp { get; init; }
    public double Value { get; init; }
}
