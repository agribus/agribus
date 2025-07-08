using Agribus.Postgres.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Agribus.Postgres.Extensions;

public static class PostgresModule
{
    public static IServiceCollection ConfigurePostgres(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services.AddDbContext<AgribusDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("Postgres"))
        );

        return services;
    }
}
