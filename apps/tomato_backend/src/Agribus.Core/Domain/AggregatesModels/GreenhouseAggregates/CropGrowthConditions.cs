namespace Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;

public class CropGrowthConditions
{
    public float? AtmosphericHumidity { get; set; }
    public float? MinimumTemperature { get; set; }
    public float? MaximumTemperature { get; set; }
}
