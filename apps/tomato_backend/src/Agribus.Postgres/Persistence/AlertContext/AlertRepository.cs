using Agribus.Core.Domain.AggregatesModels.AlertAggregates;
using Agribus.Core.Ports.Spi.AlertContext;

namespace Agribus.Postgres.Persistence.AlertContext;

public class AlertRepository(AgribusDbContext context) : IAlertRepository
{
    public async Task<Alert> AddAsync(Alert alert, CancellationToken cancellationToken)
    {
        var result = await context.Alert.AddAsync(alert, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return result.Entity;
    }

    public async Task<Alert[]> GetByGreenhouseAsync(
        Guid greenhouseId,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }
}
