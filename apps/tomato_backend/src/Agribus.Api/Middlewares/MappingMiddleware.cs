using System.Net;
using Agribus.Application.SensorUsecases;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Agribus.Api.Middlewares;

public class MappingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException exception)
        {
            await ValidationExceptionHandler.HandleExceptionAsync(context, exception);
        }
        catch (UnregisteredSensorException exception)
        {
            await SensorsExceptionHandler.HandleExceptionAsync(context, exception);
        }
        catch (Exception exception)
        {
            await HandleUnexpectedExceptionAsync(context, exception);
        }
    }

    private static async Task HandleUnexpectedExceptionAsync(
        HttpContext context,
        Exception exception
    )
    {
        var problemDetailsFactory =
            context.RequestServices.GetRequiredService<ProblemDetailsFactory>();

        var problemDetails = problemDetailsFactory.CreateProblemDetails(
            context,
            statusCode: StatusCodes.Status500InternalServerError,
            title: "UNEXCEPTED_EXCEPTION",
            detail: string.IsNullOrEmpty(exception.Message)
                ? "Please contact support if this problem persists"
                : exception.Message,
            instance: context.Request.Path
        );

        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        var logger = context.RequestServices.GetRequiredService<
            ILogger<ValidationExceptionHandler>
        >();
        logger.LogError(exception, "Unexpected error occurred: {Message}", exception.Message);

        context.Response.StatusCode =
            problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
