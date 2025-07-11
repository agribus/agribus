using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Domain.Enums;

namespace Agribus.Core.Domain.AggregatesModels.SensorAggregates;

public class Sensor : BaseEntity
{
    public required string Name { get; set; }
    public required string SourceAddress { get; set; }
    public required SensorModel SensorModel { get; set; }
    public required bool IsActive { get; set; }

    public Guid GeenhouseId { get; set; }
    public Greenhouse? Greenhouse { get; set; }

    protected Sensor() { }
}
