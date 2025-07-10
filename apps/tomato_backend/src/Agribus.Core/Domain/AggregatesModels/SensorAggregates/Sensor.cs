using Agribus.Core.Domain.Enums;

namespace Agribus.Core.Domain.AggregatesModels.SensorAggregates;

public class Sensor : BaseEntity
{
    public required string Name { get; set; }
    public required string SourceAddress { get; set; }
    public required SensorModel SensorModel { get; set; }
    public required bool IsActive { get; set; }

    protected Sensor() { }
}
