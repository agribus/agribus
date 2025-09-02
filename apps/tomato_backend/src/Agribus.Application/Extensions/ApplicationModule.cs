using Agribus.Application.AlertUsecases;
using Agribus.Application.GenericUsecases;
using Agribus.Application.GreenhouseUsecases;
using Agribus.Application.SensorUsecases;
using Agribus.Core.Ports.Api.AlertUsecases;
using Agribus.Core.Ports.Api.GenericUsecases;
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
        services.AddScoped<IGetUserGreenhousesUsecase, GetUserGreenhousesUsecase>();
        services.AddScoped<IGetGreenhouseByIdUsecase, GetGreenhouseByIdUsecase>();
        services.AddScoped<IGetAlertsByGreenhouseUsecase, GetAlertsByGreenhouseUsecase>();

        services.AddScoped<IUpdateSensorUsecase, UpdateSensorUsecase>();
        services.AddScoped<IDeleteSensorUsecase, DeleteSensorUsecase>();

        services.AddHttpClient();
        services.AddScoped<IGetHttpUsecase, GetHttpUsecase>();

        return services;
    }
}
