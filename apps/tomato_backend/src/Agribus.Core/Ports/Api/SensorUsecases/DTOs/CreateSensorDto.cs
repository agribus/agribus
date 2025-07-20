using Agribus.Core.Domain.AggregatesModels.SensorAggregates;
using Agribus.Core.Domain.Enums;

namespace Agribus.Core.Ports.Api.SensorUsecases.DTOs;

public class CreateSensorDto
{
    public required string Name { get; set; }
    public required string SourceAddress { get; set; }
    public SensorModel SensorModel { get; set; } = SensorModel.RuuviTagPro;

    public Sensor MapToSensor()
    {
        var sensor = new Sensor()
        {
            Name = this.Name,
            SourceAddress = this.SourceAddress,
            SensorModel = this.SensorModel,
        };

        return sensor;
    }
}
