using Agribus.Core.Ports.Spi.OpenMeteoContext;
using Agribus.OpenMeteo.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Agribus.OpenMeteo.Extensions;

public static class OpenMeteoModule
{
    public static IServiceCollection ConfigureOpenMeteo(this IServiceCollection services)
    {
        services.AddScoped<IForecastService, ForecastService>();
        services.AddScoped<IGeocodingApiService, GeocodingApiService>();

        return services;
    }
}
