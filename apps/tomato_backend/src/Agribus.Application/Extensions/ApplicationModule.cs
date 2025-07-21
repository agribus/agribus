using Agribus.Application.GreenhouseUsecases;
using Agribus.Application.SensorUsecases;
using Agribus.Core.Ports.Api.GreenhouseUsecases;
using Agribus.Core.Ports.Api.SensorUsecases;
using Microsoft.Extensions.DependencyInjection;

namespace Agribus.Application.Extensions;

public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<SensorDataProcessor, SensorDataProcessor>();
        services.AddScoped<ICreateGreenhouseUsecase, CreateGreenhouseUsecase>();
        services.AddScoped<IDeleteGreenhouseUsecase, DeleteGreenhouseUsecase>();
        services.AddScoped<IUpdateGreenhouseUsecase, UpdateGreenhouseUsecase>();
        services.AddScoped<IUpdateSensorUsecase, UpdateSensorUsecase>();

        return services;
    }
}
