using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;

namespace Agribus.Core.Ports.Spi.GreenhouseContext;

public interface IGreenhouseRepository
{
    Task<Greenhouse> AddAsync(
        Greenhouse greenhouse,
        string userId,
        CancellationToken cancellationToken
    );

    Task<Greenhouse?> Exists(Guid greenhouseId, string userId, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Greenhouse greenhouse, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(
        Greenhouse greenhouse,
        UpdateGreenhouseDto dto,
        CancellationToken cancellationToken
    );
}
