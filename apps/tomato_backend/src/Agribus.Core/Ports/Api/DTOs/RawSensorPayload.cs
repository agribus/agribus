using Agribus.Core.Enums;

namespace Agribus.Core.Ports.Api.DTOs;

public class RawSensorPayload
{
    public SensorType Type { get; set; } // "temperature", "humidity", "airPressure", "motion"
    public string SensorId { get; set; }
    public DateTime Timestamp { get; set; }
    public double Value { get; set; }
}
