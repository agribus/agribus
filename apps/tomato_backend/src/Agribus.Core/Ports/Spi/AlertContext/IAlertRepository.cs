using Agribus.Core.Domain.AggregatesModels.AlertAggregates;

namespace Agribus.UnitTests.AlertUsecase;

public interface IAlertRepository
{
    Task<Alert> AddAsync(Alert alert, CancellationToken cancellationToken);
}
