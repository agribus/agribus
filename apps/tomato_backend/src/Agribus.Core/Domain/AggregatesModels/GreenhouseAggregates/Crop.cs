namespace Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;

public class Crop : BaseEntity
{
    public string ScientificName { get; set; }
    public string CommonName { get; set; }
    public int Quantity { get; set; }

    protected Crop() { }
}
