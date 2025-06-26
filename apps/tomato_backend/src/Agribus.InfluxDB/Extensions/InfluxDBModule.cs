using Agribus.Core.Ports.Spi.Measurement;
using Agribus.InfluxDB.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Agribus.InfluxDB.Extensions;

public static class InfluxDBModule
{
    public static IServiceCollection ConfigureInfluxDB(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<InfluxOptions>(options =>
        {
            options.Url = configuration["InfluxDB:Url"];
            options.Token = configuration["InfluxDB:Token"];
            options.Bucket = configuration["InfluxDB:Bucket"];
            options.Org = configuration["InfluxDB:Org"];
        });

        services.AddTimeSeriesRepository();

        return services;
    }

    private static IServiceCollection AddTimeSeriesRepository(this IServiceCollection services)
    {
        services.AddSingleton<IStoreMeasurement, InfluxMeasurementStore>();

        return services;
    }
}
