using Agribus.Clerk.Models;

namespace Agribus.Clerk.Services
{
    public interface IClerkAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> SignupAsync(SignupRequest request);
        Task<bool> ValidateTokenAsync(string token);
        Task<string?> GetUserIdFromTokenAsync(string token);
    }
}
