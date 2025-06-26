using Agribus.Core.Enums;

namespace Agribus.Core.Domain.AggregatesModels;

public class SensorMeasurement
{
    public DateTime Date { get; set; }
    public double Value { get; set; }
    public string SourceAdress { get; set; }
    public SensorType Type { get; set; }
}
