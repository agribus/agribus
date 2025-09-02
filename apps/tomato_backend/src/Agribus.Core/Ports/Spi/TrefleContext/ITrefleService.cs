using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;

namespace Agribus.Core.Ports.Spi.TrefleContext;

public interface ITrefleService
{
    Task<CropGrowthConditions> GetCropIdealConditions(string scientificName);
}
