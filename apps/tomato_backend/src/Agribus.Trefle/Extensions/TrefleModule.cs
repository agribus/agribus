using Agribus.Core.Ports.Spi.TrefleContext;
using Agribus.Trefle.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Agribus.Trefle.Extensions;

public static class TrefleModule
{
    public static IServiceCollection ConfigureTrefle(this IServiceCollection services)
    {
        services.AddScoped<ITrefleService, TrefleService>();

        return services;
    }
}
