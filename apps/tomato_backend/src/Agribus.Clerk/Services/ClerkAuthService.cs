using Agribus.Clerk.Models;
using Clerk.BackendAPI;
using Clerk.BackendAPI.Models.Errors;
using Clerk.BackendAPI.Models.Operations;
using Microsoft.Extensions.Logging;
using Status = Clerk.BackendAPI.Models.Components.Status;

namespace Agribus.Clerk.Services
{
    public class ClerkAuthService : IClerkAuthService
    {
        private readonly ClerkBackendApi _clerkClient;
        private readonly ILogger<ClerkAuthService> _logger;

        public ClerkAuthService(ClerkBackendApi clerkClient, ILogger<ClerkAuthService> logger)
        {
            _clerkClient = clerkClient;
            _logger = logger;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                if (
                    string.IsNullOrWhiteSpace(request.Email)
                    || string.IsNullOrWhiteSpace(request.Password)
                )
                {
                    return AuthResponse.CreateError(
                        "Mail and password required",
                        new Dictionary<string, string[]>
                        {
                            ["validation"] = ["Mail and password required"],
                        }
                    );
                }

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
                return AuthResponse.CreateSuccess(
                    token: session.Session!.Id,
                    userId: user.Id,
                    message: "Successfully logged in"
                );
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
                var validationErrors = ValidateSignupRequest(request);
                if (validationErrors.Count != 0)
                {
                    return AuthResponse.CreateError(
                        "Invalid signup request. Please check the following fields:",
                        validationErrors
                    );
                }

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
                    userId: newUser.User!.Id,
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

        private Dictionary<string, string[]> ValidateSignupRequest(SignupRequest request)
        {
            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(request.Username))
            {
                errors["username"] = ["Username is required"];
            }
            else if (request.Username.Length < 4)
            {
                errors["username"] = ["The username must be at least 4 characters long"];
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                errors["email"] = ["Email is required"];
            }
            else if (!IsValidEmail(request.Email))
            {
                errors["email"] = ["Invalid email address"];
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                errors["password"] = ["Password is required"];
            }
            else if (request.Password.Length < 8)
            {
                errors["password"] = ["The password must be at least 8 characters long"];
            }

            if (request.Password != request.ConfirmPassword)
            {
                errors["confirmPassword"] = ["Passwords do not match"];
            }

            return errors;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
