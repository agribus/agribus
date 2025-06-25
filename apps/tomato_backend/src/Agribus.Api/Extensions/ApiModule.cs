namespace Agribus.Api.Extensions;

public static class ApiModule
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddControllers();
        services.AddProblemDetails();

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddFeatures(this IServiceCollection services)
    {
        return services;
    }
}
