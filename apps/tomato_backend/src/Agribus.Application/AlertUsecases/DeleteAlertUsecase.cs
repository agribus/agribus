using Agribus.Core.Domain.Exceptions;
using Agribus.Core.Ports.Api.AlertUsecases;
using Agribus.Core.Ports.Spi.AlertContext;

namespace Agribus.Application.AlertUsecases;

public class DeleteAlertUsecase(IAlertRepository alertRepository) : IDeleteAlertUsecase
{
    public async Task<bool> Handle(Guid alertId, string userId, CancellationToken cancellationToken)
    {
        var alertExists = await alertRepository.Exists(alertId, userId, cancellationToken);
        if (alertExists is null)
            throw new NotFoundEntityException("The specified alert does not exist");

        return await alertRepository.DeleteAsync(alertId, cancellationToken);
    }
}
