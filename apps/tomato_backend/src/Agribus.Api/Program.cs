using Agribus.Api.Extensions;
using Agribus.Api.Middlewares;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddPresentation().AddFeatures().AddInfrastructure();
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5001); // <-- accepte les connexions externes
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
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