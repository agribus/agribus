using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Domain.AggregatesModels.SensorAggregates;
using Agribus.Core.Ports.Api.SensorUsecases.DTOs;

namespace Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;

public class CreateGreenhouseDto
{
    public required string Name { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public required List<Crop> Crops { get; set; }
    public required List<CreateSensorDto> Sensors { get; set; }

    public Greenhouse MapToGreenhouse()
    {
        var greenhouse = new Greenhouse()
        {
            Name = Name,
            City = City,
            Country = Country,
            Crops = Crops,
        };

        var sensorEntities = Sensors.Select(s => s.MapToSensor());
        greenhouse.AddSensors(sensorEntities);

        return greenhouse;
    }
}
