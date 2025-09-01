using Agribus.Core.Domain.AggregatesModels.AlertAggregates;

namespace Agribus.Core.Ports.Spi.AlertContext;

public interface IAlertRepository
{
    Task<Alert> AddAsync(Alert alert, CancellationToken cancellationToken);
    Task<Alert[]> GetByGreenhouseAsync(Guid greenhouseId, CancellationToken cancellationToken);
}
