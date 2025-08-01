using Agribus.Clerk.Services;
using Agribus.Clerk.Validators;
using Agribus.Core.Ports.Spi.AuthContext;
using Clerk.BackendAPI;
using FluentValidation;
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

            services.AddHttpContextAccessor();

            services.AddSingleton(provider => new ClerkBackendApi(bearerAuth: secretKey));

            services.AddScoped<IAuthService, AuthService>();
            services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>(
                ServiceLifetime.Singleton
            );
            services.AddValidatorsFromAssemblyContaining<SignupRequestValidator>(
                ServiceLifetime.Singleton
            );

            return services;
        }
    }
}
