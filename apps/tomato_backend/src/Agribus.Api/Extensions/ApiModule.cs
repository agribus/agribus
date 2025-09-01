using Agribus.Core.Ports.Api.Validators;
using FluentValidation;

namespace Agribus.Api.Extensions;

public static class ApiModule
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddControllers();
        services.AddCors(options =>
        {
            options.AddPolicy(
                "AllowAll",
                builder =>
                {
                    builder
                        .SetIsOriginAllowed(_ => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                }
            );
        });
        services.Configure<CookiePolicyOptions>(o =>
        {
            o.MinimumSameSitePolicy = SameSiteMode.None;
            o.Secure = CookieSecurePolicy.Always;
            o.OnAppendCookie = ctx => SetSameSiteNone(ctx.CookieOptions);
            o.OnDeleteCookie = ctx => SetSameSiteNone(ctx.CookieOptions);

            static void SetSameSiteNone(CookieOptions opts)
            {
                opts.SameSite = SameSiteMode.None;
                opts.Secure = true;
            }
        });

        return services;
    }
}
