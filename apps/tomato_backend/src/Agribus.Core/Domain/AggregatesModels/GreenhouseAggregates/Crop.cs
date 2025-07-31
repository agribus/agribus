namespace Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;

public record Crop
{
    public required string ScientificName { get; init; }
    public required string CommonName { get; init; }
    public required int Quantity { get; init; }
    public required DateTime PlantingDate { get; init; }
}
