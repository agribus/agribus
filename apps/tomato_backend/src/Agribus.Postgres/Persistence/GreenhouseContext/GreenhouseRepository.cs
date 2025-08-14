using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;
using Agribus.Core.Ports.Spi.GreenhouseContext;

namespace Agribus.Postgres.Persistence.GreenhouseContext;

public class GreenhouseRepository(AgribusDbContext context) : IGreenhouseRepository
{
    public async Task<Greenhouse> AddAsync(
        Greenhouse greenhouse,
        CancellationToken cancellationToken
    )
    {
        var result = await context.Greenhouse.AddAsync(greenhouse, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return result.Entity;
    }

    public async Task<Greenhouse?> Exists(
        Guid greenhouseId,
        string userId,
        CancellationToken cancellationToken
    )
    {
        var greenhouse = await context
            .Greenhouse.Where(g => g.UserId == userId)
            .FirstOrDefaultAsync(g => g.Id == greenhouseId, cancellationToken);

        return greenhouse;
    }

    public async Task<bool> DeleteAsync(Greenhouse greenhouse, CancellationToken cancellationToken)
    {
        context.Greenhouse.Remove(greenhouse);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateAsync(
        Greenhouse greenhouse,
        UpdateGreenhouseDto dto,
        CancellationToken cancellationToken
    )
    {
        greenhouse.Update(dto);
        context.Greenhouse.Update(greenhouse);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<GreenhouseListItemDto[]> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken
    )
    {
        return await context
            .Greenhouse.Where(g => g.UserId == userId)
            .OrderByDescending(g => g.CreatedAt)
            .Select(g => new GreenhouseListItemDto(g.Id, g.Name))
            .ToArrayAsync(cancellationToken);
    }
}
