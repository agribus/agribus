namespace Agribus.Core.Domain.AggregatesModels.AuthAggregates
{
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class SignupRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class PasswordChangeRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }

    public sealed class AuthResponse
    {
        public bool Success { get; set; } = false;

        public string? Token { get; set; }
        public string? Message { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; } = new();

        public static AuthResponse CreateSuccess(string? token = null, string? message = null)
        {
            return new AuthResponse
            {
                Success = true,
                Token = token,
                Message = message ?? "Successful operation",
            };
        }

        public static AuthResponse CreateError(
            string message,
            Dictionary<string, string[]>? errors = null
        )
        {
            return new AuthResponse
            {
                Success = false,
                Message = message,
                Errors = errors ?? new Dictionary<string, string[]>(),
            };
        }
    }
}
