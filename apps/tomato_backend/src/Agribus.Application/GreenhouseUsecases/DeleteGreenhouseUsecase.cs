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
        Guid fakeUserId,
        CancellationToken cancellationToken
    )
    {
        return await greenhouseRepository.DeleteAsync(greenhouseId, fakeUserId, cancellationToken);
    }
}
