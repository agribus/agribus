using Agribus.Core.Domain.AggregatesModels.SensorAggregates;

namespace Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;

public class Greenhouse : BaseEntity
{
    public string Name { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public List<Crop> Crops { get; set; } = new();

    public IReadOnlyCollection<Sensor> Sensors => _sensors.AsReadOnly();
    private readonly List<Sensor> _sensors = [];

    protected Greenhouse() { }
}
