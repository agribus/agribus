using Microsoft.Extensions.DependencyInjection;

namespace Agribus.Application.Extensions;

public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<SensorDataProcessor, SensorDataProcessor>();
        return services;
    }
}