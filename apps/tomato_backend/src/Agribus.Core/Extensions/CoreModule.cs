using Agribus.Core.Ports.Api.ParseSensorData;
using Agribus.Core.Ports.Api.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Agribus.Core.Extensions;

public static class CoreModule
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddScoped<IParseSensorData, ParseSensorData>();
        services.AddValidatorsFromAssemblyContaining<RawSensorPayloadValidator>(
            ServiceLifetime.Singleton
        );
        return services;
    }
}
