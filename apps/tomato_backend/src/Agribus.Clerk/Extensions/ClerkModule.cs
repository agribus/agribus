using Agribus.Clerk.Services;
using Clerk.BackendAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Agribus.Clerk.Extensions
{
    public static class ClerkModule
    {
        public static IServiceCollection AddClerk(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var secretKey = configuration["Clerk:SecretKey"];

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new InvalidOperationException("Clerk SecretKey is not configured");
            }

            services.AddSingleton(provider => new ClerkBackendApi(bearerAuth: secretKey));

            services.AddScoped<IClerkAuthService, ClerkAuthService>();

            return services;
        }
    }
}
