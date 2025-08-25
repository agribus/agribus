using Agribus.Core.Domain.AggregatesModels.OpenMeteoAggregates;

namespace Agribus.Core.Ports.Spi.OpenMeteoContext;

public interface IForecastService
{
    Task<List<ForecastHourly>> GetForecastAsync(string lat, string lon);
}
