using System.Text.Json.Serialization;

namespace Agribus.Core.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SensorType
{
    Temperature,
    Humidity,
    Pressure,
    Motion,
    Rssi,
    Neighbors,
    Unknown,
}
