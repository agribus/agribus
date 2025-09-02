using Agribus.Core.Domain.AggregatesModels.SensorAggregates;
using Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;

namespace Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;

public class Greenhouse : BaseEntity
{
    public required string Name { get; set; }
    public required string Country { get; set; }
    public required string City { get; set; }
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
    public required string UserId { get; set; }
    public List<Crop> Crops { get; set; } = new();
    public List<Sensor> Sensors { get; set; } = new();

    public void AddSensors(IEnumerable<Sensor> sensors)
    {
        Sensors.AddRange(
            sensors.Select(s =>
            {
                s.Greenhouse = this;
                return s;
            })
        );
    }

    public void AddCoordinate(string latitude, string longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public void Update(UpdateGreenhouseDto dto)
    {
        Name = dto.Name ?? Name;
        Country = dto.Country ?? Country;
        City = dto.City ?? City;
        if (dto.Crops is not null)
            Crops = dto.Crops.Select(c => c.MapToCrop()).ToList();

        UpdateLastModified();
    }
}
