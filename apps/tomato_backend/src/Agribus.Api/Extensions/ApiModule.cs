using Agribus.Core.Features;
using Agribus.Core.Ports.Api.Interfaces;
using Agribus.Core.Ports.Api.Validators;
using FluentValidation;

namespace Agribus.Api.Extensions;

public static class ApiModule
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddControllers();
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // API
        services.AddScoped<IParseSensorData, ParseAndStoreSensorDataFeature>();

        services.AddValidatorsFromAssemblyContaining<RawSensorPayloadValidator>(
            ServiceLifetime.Singleton
        );

        return services;
    }

    public static IServiceCollection AddFeatures(this IServiceCollection services)
    {
        return services;
    }
}
