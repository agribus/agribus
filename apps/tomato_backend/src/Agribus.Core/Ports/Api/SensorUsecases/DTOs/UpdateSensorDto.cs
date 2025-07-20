using Agribus.Core.Domain.AggregatesModels.SensorAggregates;
using Agribus.Core.Domain.Enums;

namespace Agribus.Core.Ports.Api.SensorUsecases.DTOs;

public class UpdateSensorDto
{
    public required Guid Id { get; set; }
    public string? Name { get; set; }
    public SensorModel? Model { get; set; }
    public bool? IsActive { get; set; }
}
