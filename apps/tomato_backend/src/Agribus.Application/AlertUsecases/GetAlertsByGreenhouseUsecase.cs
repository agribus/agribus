using Agribus.Core.Domain.AggregatesModels.AlertAggregates;
using Agribus.Core.Domain.Exceptions;
using Agribus.Core.Ports.Api.AlertUsecases;
using Agribus.Core.Ports.Spi.AlertContext;
using Agribus.Core.Ports.Spi.GreenhouseContext;

namespace Agribus.Application.AlertUsecases;

public class GetAlertsByGreenhouseUsecase(
    IAlertRepository alertRepository,
    IGreenhouseRepository greenhouseRepository
) : IGetAlertsByGreenhouseUsecase
{
    public async Task<IEnumerable<Alert>> Handle(
        Guid greenhouseId,
        string userId,
        CancellationToken cancellationToken
    )
    {
        var greenhouseExists = await greenhouseRepository.Exists(
            greenhouseId,
            userId,
            cancellationToken
        );
        if (greenhouseExists is null)
            throw new NotFoundEntityException("The specified greenhouse does not exist");

        var result = await alertRepository.GetByGreenhouseAsync(greenhouseId, cancellationToken);
        return result;
    }
}
