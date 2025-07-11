namespace Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;

public record Crop
{
    public string ScientificName { get; init; }
    public string CommonName { get; init; }
    public int Quantity { get; init; }
}
