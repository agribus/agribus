using Agribus.Core.Ports.Api.GreenhouseUsecases;
using Agribus.Core.Ports.Spi.GreenhouseContext;

namespace Agribus.Application.GreenhouseUsecases;

public class DeleteGreenhouseUsecase(
    // IAuthContextService authContext,
    IGreenhouseRepository greenhouseRepository
) : IDeleteGreenhouseUsecase
{
    public async Task<bool> Handle(
        Guid greenhouseId,
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        var greenhouse = await greenhouseRepository.Exists(greenhouseId, userId, cancellationToken);
        if (greenhouse is null)
            return false;

        return await greenhouseRepository.DeleteAsync(greenhouse, cancellationToken);
    }
}
