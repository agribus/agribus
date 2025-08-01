using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;

namespace Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;

public class UpdateGreenhouseDto
{
    public string? Name { get; set; }
    public string? Country { get; set; }

    public string? City { get; set; }
    public List<UpdateCropDto>? Crops { get; set; }
}
