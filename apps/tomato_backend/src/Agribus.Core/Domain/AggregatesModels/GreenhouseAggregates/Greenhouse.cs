using Agribus.Core.Domain.AggregatesModels.SensorAggregates;

namespace Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;

public class Greenhouse : BaseEntity
{
    public required string Name { get; set; }
    public required string Country { get; set; }
    public required string City { get; set; }
    public List<Crop> Crops { get; set; } = new();

    public IReadOnlyCollection<Sensor> Sensors => _sensors.AsReadOnly();
    private readonly List<Sensor> _sensors = [];

    public void AddSensor(IEnumerable<Sensor> sensors)
    {
        _sensors.AddRange(sensors);
    }
}
