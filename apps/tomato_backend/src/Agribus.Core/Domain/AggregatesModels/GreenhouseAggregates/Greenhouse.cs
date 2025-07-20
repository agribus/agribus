using Agribus.Core.Domain.AggregatesModels.SensorAggregates;
using Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;

namespace Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;

public class Greenhouse : BaseEntity
{
    public required string Name { get; set; }
    public required string Country { get; set; }
    public required string City { get; set; }
    public List<Crop> Crops { get; set; } = new();

    public IReadOnlyCollection<Sensor> Sensors => _sensors.AsReadOnly();
    private readonly List<Sensor> _sensors = [];

    public void AddSensors(IEnumerable<Sensor> sensors)
    {
        _sensors.AddRange(
            sensors.Select(s =>
            {
                s.Greenhouse = this;
                return s;
            })
        );
    }

    public void Update(UpdateGreenhouseDto dto)
    {
        Name = dto.Name ?? Name;
        Country = dto.Country ?? Country;
        City = dto.City ?? City;
        Crops = dto.Crops ?? Crops;
        // Todo: Sensors individual update
    }
}
