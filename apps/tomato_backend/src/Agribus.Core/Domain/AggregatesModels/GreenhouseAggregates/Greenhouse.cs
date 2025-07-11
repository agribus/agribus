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

    // Uncomment to associate a User with the Greenhouse
    // public Guid UserId { get; set; }
    // public User? User { get; set; }

    protected Greenhouse() { }
}
