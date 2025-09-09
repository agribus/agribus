using Agribus.Core.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Agribus.Api.Middlewares;

public class NotFoundExceptionHandler
{
    public static async Task HandleExceptionAsync(
        HttpContext context,
        NotFoundEntityException exception
    )
    {
        var problemDetailsFactory =
            context.RequestServices.GetRequiredService<ProblemDetailsFactory>();

        var problemDetails = problemDetailsFactory.CreateProblemDetails(
            context,
            statusCode: StatusCodes.Status404NotFound,
            title: "RESOURCE_NOT_FOUND",
            detail: string.IsNullOrEmpty(exception.Message)
                ? "The requested resource was not found"
                : exception.Message,
            instance: context.Request.Path
        );

        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        var logger = context.RequestServices.GetRequiredService<ILogger<NotFoundEntityException>>();
        logger.LogWarning(exception, "Resource not found: {Message}", exception.Message);

        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status404NotFound;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
