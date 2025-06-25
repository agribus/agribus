using System.Text.Json.Serialization;

namespace Agribus.Core.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SensorType
{
    Temperature,
    Humidity,
    Pressure,
    Motion,
}
