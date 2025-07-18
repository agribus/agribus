namespace Agribus.Core.Ports.Spi.AuthContext;

public interface IAuthContextService
{
    Guid GetCurrentUserId(); // TODO: Clerk implementation
}
