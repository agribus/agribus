using Agribus.Clerk.Services;

namespace Agribus.Api.Middlewares
{
    public class ClerkAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public ClerkAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IClerkAuthService clerkAuthService)
        {
            var path = context.Request.Path.Value?.ToLower();

            if (IsPublicEndpoint(path))
            {
                await _next(context);
                return;
            }

            var token = ExtractTokenFromRequest(context);
            if (string.IsNullOrWhiteSpace(token))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Missing Token");
                return;
            }

            var isValid = await clerkAuthService.ValidateTokenAsync(token);
            if (!isValid)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid Token");
                return;
            }

            var userId = await clerkAuthService.GetUserIdFromTokenAsync(token);
            if (!string.IsNullOrWhiteSpace(userId))
            {
                context.Items["UserId"] = userId;
            }

            await _next(context);
        }

        private static bool IsPublicEndpoint(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            var publicEndpoints = new[]
            {
                Endpoints.User.Login,
                Endpoints.User.Signup,
                Endpoints.Ping.Index,
                "/swagger",
            };

            return publicEndpoints.Any(path.StartsWith);
        }

        private static string? ExtractTokenFromRequest(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer "))
            {
                return authHeader["Bearer ".Length..].Trim();
            }

            return context.Request.Cookies.TryGetValue("clerk-session", out var sessionToken)
                ? sessionToken
                : null;
        }
    }
}
