using Agribus.Application.SensorUsecases;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Agribus.Api.Middlewares;

public class SensorsExceptionHandler
{
    public static async Task HandleExceptionAsync(
        HttpContext context,
        UnregisteredSensorException exception
    )
    {
        var problemDetailsFactory =
            context.RequestServices.GetRequiredService<ProblemDetailsFactory>();
        var modelStateDictionary = new ModelStateDictionary();

        var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
            context,
            modelStateDictionary,
            statusCode: StatusCodes.Status400BadRequest,
            title: "BAD_REQUEST_EXCEPTION",
            detail: "Please refer to the errors property for additional details"
        );

        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
