using Agribus.Core.Domain.Enums;

namespace Agribus.Core.Domain.AggregatesModels.SensorAggregates;

public class SensorMeasurement
{
    public required DateTime Date { get; set; }
    public required double Value { get; set; }
    public required string SourceAdress { get; set; }
    public required SensorType Type { get; set; }
}
