using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Domain.AggregatesModels.SensorAggregates;

namespace Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;

public class CreateGreenhouseDto
{
    public required string Name { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public required List<Crop> Crops { get; set; }
    public required List<Sensor> Sensors { get; set; }

    public Greenhouse MapToGreenhouse()
    {
        var greenhouse = new Greenhouse()
        {
            Name = Name,
            City = City,
            Country = Country,
            Crops = Crops,
        };

        greenhouse.AddSensor(Sensors);

        return greenhouse;
    }
}
