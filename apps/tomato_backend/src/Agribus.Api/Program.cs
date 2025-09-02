using Agribus.Api.Extensions;
using Agribus.Api.Middlewares;
using Agribus.Application.Extensions;
using Agribus.Clerk.Extensions;
using Agribus.Core.Extensions;
using Agribus.InfluxDB.Extensions;
using Agribus.OpenMeteo.Extensions;
using Agribus.Postgres.Extensions;
using Agribus.Trefle.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder
    .Services.AddPresentation()
    .AddCore()
    .ConfigureInfluxDB(config)
    .AddClerk(config)
    .AddApplication()
    .ConfigurePostgres(config)
    .ConfigureOpenMeteo()
    .ConfigureTrefle();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "openapi/{documentName}.json"; // Ensure correct route
    });
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/openapi/v1.json", "Agribus API"));

    app.MapScalarApiReference();
}

// app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseCookiePolicy();
app.UseMiddleware<ClerkAuthenticationMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<MappingMiddleware>();

app.Lifetime.ApplicationStarted.Register(() =>
{
    var addresses = app.Urls;
    foreach (var addr in addresses)
    {
        Console.WriteLine($"Listening on: {addr}");
    }
});

app.Run();
