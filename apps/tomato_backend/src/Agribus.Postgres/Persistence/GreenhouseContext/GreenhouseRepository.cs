using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;
using Agribus.Core.Ports.Spi.GreenhouseContext;

namespace Agribus.Postgres.Persistence.GreenhouseContext;

public class GreenhouseRepository(AgribusDbContext context) : IGreenhouseRepository
{
    public async Task<Greenhouse> AddAsync(
        Greenhouse greenhouse,
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        var result = await context.Greenhouse.AddAsync(greenhouse, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return result.Entity;
    }

    public async Task<Greenhouse?> Exists(
        Guid greenhouseId,
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        var greenhouse = await context.Greenhouse
        // .Where(g => g.UserId == userId)
        .FirstOrDefaultAsync(g => g.Id == greenhouseId, cancellationToken);

        return greenhouse;
    }

    public async Task<bool> DeleteAsync(Greenhouse greenhouse, CancellationToken cancellationToken)
    {
        context.Greenhouse.Remove(greenhouse);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> UpdateAsync(
        Greenhouse greenhouse,
        UpdateGreenhouseDto dto,
        CancellationToken cancellationToken
    )
    {
        greenhouse.Update(dto);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}
