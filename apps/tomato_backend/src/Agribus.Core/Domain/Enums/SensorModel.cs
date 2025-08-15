using System.Text.Json.Serialization;

namespace Agribus.Core.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SensorModel
{
    RuuviTag,
    RuuviTagPro,
    Unknown,
}
