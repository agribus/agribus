using Agribus.Core.Domain.AggregatesModels.AlertAggregates;

namespace Agribus.Core.Ports.Spi.AlertContext;

public interface IAlertRepository
{
    Task<Alert> AddAsync(Alert alert, CancellationToken cancellationToken);
    Task<IEnumerable<Alert>> GetByGreenhouseAsync(
        Guid greenhouseId,
        CancellationToken cancellationToken
    );
    Task<Alert?> Exists(Guid alertId, string userId, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid alertId, CancellationToken cancellationToken);
}
