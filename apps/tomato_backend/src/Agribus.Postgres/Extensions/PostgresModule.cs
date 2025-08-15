using Agribus.Core.Ports.Spi.GreenhouseContext;
using Agribus.Core.Ports.Spi.SensorContext;
using Agribus.Postgres.Persistence;
using Agribus.Postgres.Persistence.GreenhouseContext;
using Agribus.Postgres.Persistence.SensorContext;
using Microsoft.Extensions.DependencyInjection;

namespace Agribus.Postgres.Extensions;

public static class PostgresModule
{
    public static IServiceCollection ConfigurePostgres(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services
            .AddDbContext<AgribusDbContext>(options =>
                options
                    .UseNpgsql(config.GetConnectionString("Postgres"))
                    .UseSnakeCaseNamingConvention()
            )
            .AddRepositories();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IGreenhouseRepository, GreenhouseRepository>();
        services.AddScoped<ISensorRepository, SensorRepository>();

        return services;
    }
}
