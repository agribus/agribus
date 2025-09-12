using Agribus.Clerk.Validators;
using Agribus.Core.Domain.AggregatesModels.AuthAggregates;
using Agribus.Core.Ports.Spi.AuthContext;
using Clerk.BackendAPI;
using Clerk.BackendAPI.Models.Errors;
using Clerk.BackendAPI.Models.Operations;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Status = Clerk.BackendAPI.Models.Components.Status;

namespace Agribus.Clerk.Services
{
    public class AuthService : IAuthService
    {
        private readonly ClerkBackendApi _clerkClient;
        private readonly ILogger<AuthService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LoginRequestValidator _loginValidator;
        private readonly SignupRequestValidator _signupValidator;

        public AuthService(
            ClerkBackendApi clerkClient,
            ILogger<AuthService> logger,
            IHttpContextAccessor httpContextAccessor,
            LoginRequestValidator loginValidator,
            SignupRequestValidator signupValidator
        )
        {
            _clerkClient = clerkClient;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _loginValidator = loginValidator;
            _signupValidator = signupValidator;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                await _loginValidator.ValidateAndThrowAsync(request);

                var usersList = await _clerkClient.Users.ListAsync(
                    new() { EmailAddress = [request.Email] }
                );

                if (usersList.UserList == null || usersList.UserList.Count == 0)
                    return AuthResponse.CreateError(
                        "User not found",
                        new Dictionary<string, string[]> { ["auth"] = ["Email is incorrect"] }
                    );
                var user = usersList.UserList.FirstOrDefault()!;

                try
                {
                    await _clerkClient.Users.VerifyPasswordAsync(
                        userId: user.Id,
                        requestBody: new VerifyPasswordRequestBody { Password = request.Password }
                    );
                }
                catch
                {
                    return AuthResponse.CreateError(
                        "Incorrect credentials.",
                        new Dictionary<string, string[]> { ["auth"] = ["Password is incorrect"] }
                    );
                }

                var session = await _clerkClient.Sessions.CreateAsync(new() { UserId = user.Id });

                SetAuthCookie(session.Session!.Id);

                return AuthResponse.CreateSuccess(message: "Successfully logged in");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la connexion pour {Email}", request.Email);
                return AuthResponse.CreateError(
                    "Connection error. Please try again later.",
                    new Dictionary<string, string[]> { ["server"] = ["An error has occured"] }
                );
            }
        }

        public async Task<AuthResponse> SignupAsync(SignupRequest request)
        {
            try
            {
                await _signupValidator.ValidateAndThrowAsync(request);

                var userEmailCheckList = await _clerkClient.Users.ListAsync(
                    new() { EmailAddress = [request.Email] }
                );

                if (userEmailCheckList.UserList is { Count: > 0 })
                    return AuthResponse.CreateError(
                        "User conflict",
                        new Dictionary<string, string[]> { ["auth"] = ["Email is already taken"] }
                    );
                var userUsernameCheckList = await _clerkClient.Users.ListAsync(
                    new() { Username = [request.Username] }
                );

                if (userUsernameCheckList.UserList is { Count: > 0 })
                    return AuthResponse.CreateError(
                        "User conflict",
                        new Dictionary<string, string[]>
                        {
                            ["auth"] = ["Username is already taken"],
                        }
                    );

                var newUser = await _clerkClient.Users.CreateAsync(
                    new()
                    {
                        EmailAddress = [request.Email],
                        Username = request.Username,
                        Password = request.Password,
                    }
                );

                return AuthResponse.CreateSuccess(
                    message: "Account successfully created. Please check your email to verify your account."
                );
            }
            catch (ClerkErrors ex)
            {
                _logger.LogError(
                    ex,
                    "Erreur lors de la création du compte pour {Email}",
                    request.Email
                );
                return AuthResponse.CreateError(
                    "Account creation error. Please try again later.",
                    new Dictionary<string, string[]> { ["server"] = ["An error has occured"] }
                );
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var session = await _clerkClient.Sessions.GetAsync(sessionId: token);
                return session.Session!.Status == Status.Active;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string?> GetUserIdFromTokenAsync(string token)
        {
            try
            {
                var session = await _clerkClient.Sessions.GetAsync(sessionId: token);
                return session.Session!.UserId;
            }
            catch
            {
                return null;
            }
        }

        public string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext.Items["UserId"]!.ToString()!;
        }

        public void LogoutAsync()
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete("auth_token");
        }

        public string? GetToken()
        {
            return _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(
                "auth_token",
                out var token
            )
                ? token
                : null;
        }

        private void SetAuthCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                Path = "/",
            };

            _httpContextAccessor.HttpContext?.Response.Cookies.Append(
                "auth_token",
                token,
                cookieOptions
            );
        }
    }
}
