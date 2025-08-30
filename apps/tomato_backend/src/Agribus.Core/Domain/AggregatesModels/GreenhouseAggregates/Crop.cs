namespace Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;

public record Crop
{
    public required string ScientificName { get; init; }
    public required string CommonName { get; init; }
    public required int Quantity { get; init; }
    public required DateTime PlantingDate { get; init; }
    public string? ImageUrl { get; init; }

    public CropGrowthConditions? IdealConditions { get; set; }

    public void AddCropGrowthConditions(CropGrowthConditions cropGrowthConditions)
    {
        IdealConditions ??= new CropGrowthConditions();
        IdealConditions.AtmosphericHumidity = cropGrowthConditions.AtmosphericHumidity;
        IdealConditions.MinimumTemperature = cropGrowthConditions.MinimumTemperature;
        IdealConditions.MaximumTemperature = cropGrowthConditions.MaximumTemperature;
    }
}
