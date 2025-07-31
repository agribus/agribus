using Agribus.Core.Domain.AggregatesModels.SensorAggregates;
using Agribus.Core.Domain.Enums;

namespace Agribus.Core.Ports.Api.SensorUsecases.DTOs;

public class CreateSensorDto
{
    public required string Name { get; set; }
    public required string SourceAddress { get; set; }
    public SensorModel SensorModel { get; set; } = SensorModel.RuuviTagPro;
    public bool? IsActive { get; set; }

    public Sensor MapToSensor()
    {
        var sensor = new Sensor()
        {
            Name = Name,
            SourceAddress = SourceAddress,
            Model = SensorModel,
            IsActive = IsActive,
        };

        return sensor;
    }
}
