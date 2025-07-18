using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;

namespace Agribus.Core.Ports.Spi.GreenhouseContext;

public interface IGreenhouseRepository
{
    Task<Greenhouse> AddAsync(
        Greenhouse greenhouse,
        Guid userId,
        CancellationToken cancellationToken
    );
}
