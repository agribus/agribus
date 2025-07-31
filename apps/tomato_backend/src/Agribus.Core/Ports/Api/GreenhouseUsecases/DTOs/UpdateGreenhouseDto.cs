using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Ports.Api.SensorUsecases.DTOs;

namespace Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;

public class UpdateGreenhouseDto
{
    public string? Name { get; init; }
    public string? Country { get; init; }

    public string? City { get; init; }
    public List<Crop>? Crops { get; init; }
}
