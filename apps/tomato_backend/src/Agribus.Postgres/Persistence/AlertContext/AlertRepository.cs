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

    public async Task<IEnumerable<Alert>> GetByGreenhouseAsync(
        Guid greenhouseId,
        CancellationToken cancellationToken
    )
    {
        return await context
            .Alert.Where(x => x.GreenhouseId == greenhouseId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Alert?> Exists(
        Guid alertId,
        string userId,
        CancellationToken cancellationToken
    )
    {
        var alert = await context
            .Alert.Where(x => x.Id == alertId && x.Greenhouse.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);
        return alert;
    }

    public async Task<bool> DeleteAsync(Guid alertId, CancellationToken cancellationToken)
    {
        var entity = await context.Alert.FirstOrDefaultAsync(
            x => x.Id == alertId,
            cancellationToken
        );
        if (entity == null)
            return false;

        context.Alert.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
