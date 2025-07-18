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
        var result = context.Greenhouse.AddAsync(greenhouse, cancellationToken);
        return result.Result.Entity;
    }
}
