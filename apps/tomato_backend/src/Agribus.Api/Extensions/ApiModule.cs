using Agribus.Core.Ports.Api.Validators;
using Agribus.Core.Tests;
using FluentValidation;

namespace Agribus.Api.Extensions;

public static class ApiModule
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddControllers();

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // API
        services.AddScoped<IParseSensorData, ParseSensorData>();

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
