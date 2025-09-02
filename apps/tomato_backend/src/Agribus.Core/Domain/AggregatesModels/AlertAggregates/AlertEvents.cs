using System.Text.Json.Serialization;
using Agribus.Core.Domain.AggregatesModels.SensorAggregates;

namespace Agribus.Core.Domain.AggregatesModels.AlertAggregates;

public class AlertEvents : BaseEntity
{
    public required double MeasureValue { get; init; }
    public DateTimeOffset OccuredAt { get; init; }

    public Guid AlertId { get; init; }

    [JsonIgnore]
    public Alert Alert { get; init; }

    public Guid? SensorId { get; init; }

    [JsonIgnore]
    public Sensor? Sensor { get; init; }
}
