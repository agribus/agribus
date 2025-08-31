using System.Text.Json.Serialization;
using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Domain.Enums;

namespace Agribus.Core.Domain.AggregatesModels.AlertAggregates;

public class Alert : BaseEntity
{
    public required string Name { get; init; }
    public required bool Enabled { get; init; } = true;
    public required double ThresholdValue { get; init; }
    public required double RangeMinValue { get; init; }
    public required double RangeMaxValue { get; init; }

    public required SensorType MeasureType { get; init; }
    public required AlertRuleType RuleType { get; init; }

    [JsonIgnore]
    public Guid GreenhouseId { get; init; }

    [JsonIgnore]
    public Greenhouse Greenhouse { get; init; }

    public ICollection<AlertEvents> AlertEvents => _events.AsReadOnly();
    private readonly List<AlertEvents> _events = [];
}
