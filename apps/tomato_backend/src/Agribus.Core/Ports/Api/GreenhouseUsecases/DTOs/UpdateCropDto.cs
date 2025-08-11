using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;

namespace Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;

public class UpdateCropDto
{
    public required string ScientificName { get; set; }
    public required string CommonName { get; set; }
    public required int Quantity { get; set; }
    public required DateTime PlantingDate { get; set; }

    public Crop MapToCrop()
    {
        return new Crop
        {
            ScientificName = ScientificName,
            CommonName = CommonName,
            Quantity = Quantity,
            PlantingDate = PlantingDate,
        };
    }
}
