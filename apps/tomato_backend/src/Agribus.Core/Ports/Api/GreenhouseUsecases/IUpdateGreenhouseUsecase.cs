using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;

namespace Agribus.Core.Ports.Api.GreenhouseUsecases;

public interface IUpdateGreenhouseUsecase
{
    Task<bool?> Handle(
        Guid greenhouseId,
        Guid userId,
        UpdateGreenhouseDto dto,
        CancellationToken cancellationToken
    );
}
