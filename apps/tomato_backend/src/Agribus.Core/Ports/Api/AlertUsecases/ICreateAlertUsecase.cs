using Agribus.Core.Domain.AggregatesModels.AlertAggregates;
using Agribus.Core.Ports.Api.AlertUsecases.DTOs;

namespace Agribus.Core.Ports.Api.AlertUsecases;

public interface ICreateAlertUsecase
{
    Task<Alert> Handle(CreateAlertDto dto, string userId, CancellationToken ct);
}
