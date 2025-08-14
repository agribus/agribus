using Agribus.Core.Domain.AggregatesModels.AuthAggregates;

namespace Agribus.Core.Ports.Spi.AuthContext;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> SignupAsync(SignupRequest request);
    Task<bool> ValidateTokenAsync(string token);
    Task<string?> GetUserIdFromTokenAsync(string token);
    void LogoutAsync();
    string GetCurrentUserId();
    string GetToken();
}
