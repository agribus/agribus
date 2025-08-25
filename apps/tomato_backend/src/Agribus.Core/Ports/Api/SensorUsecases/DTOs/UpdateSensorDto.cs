using Agribus.Core.Domain.AggregatesModels.SensorAggregates;
using Agribus.Core.Domain.Enums;

namespace Agribus.Core.Ports.Api.SensorUsecases.DTOs;

public class UpdateSensorDto
{
    public string? Name { get; set; }
    public SensorModel? Model { get; set; }
    public bool? IsActive { get; set; }
}

public class UpdateSensorFromGreenhouseDto : UpdateSensorDto
{
    public Guid Id { get; set; }
}
