using Agribus.Core.Domain.AggregatesModels.AlertAggregates;
using Agribus.Core.Domain.Enums;

namespace Agribus.Core.Ports.Api.AlertUsecases.DTOs;

public class CreateAlertDto
{
    public Guid GreenhouseId { get; set; }
    public string Name { get; set; }
    public SensorType MeasureType { get; set; }
    public AlertRuleType RuleType { get; set; }
    public double? ThresholdValue { get; set; }
    public double? RangeMinValue { get; set; }
    public double? RangeMaxValue { get; set; }
    public bool Enabled { get; set; }

    public Alert MapToAlert()
    {
        return new Alert()
        {
            Name = Name,
            MeasureType = MeasureType,
            ThresholdValue = ThresholdValue,
            RangeMaxValue = RangeMaxValue,
            RangeMinValue = RangeMinValue,
            Enabled = Enabled,
            RuleType = RuleType,
            GreenhouseId = GreenhouseId,
        };
    }
}
