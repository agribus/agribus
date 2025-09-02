using Agribus.Core.Domain.AggregatesModels.AlertAggregates;

namespace Agribus.Core.Ports.Api.AlertUsecases;

public interface IGetAlertsByGreenhouseUsecase
{
    Task<IEnumerable<Alert>> Handle(
        Guid greenhouseId,
        string userId,
        CancellationToken cancellationToken
    );
}
