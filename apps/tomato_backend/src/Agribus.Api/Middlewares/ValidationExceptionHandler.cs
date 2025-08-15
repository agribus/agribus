using FluentValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Agribus.Api.Middlewares;

public class ValidationExceptionHandler
{
    public static async Task HandleExceptionAsync(
        HttpContext context,
        ValidationException exception
    )
    {
        var problemDetailsFactory =
            context.RequestServices.GetRequiredService<ProblemDetailsFactory>();
        var modelStateDictionary = new ModelStateDictionary();
        foreach (var error in exception.Errors)
        {
            modelStateDictionary.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
            context,
            modelStateDictionary,
            statusCode: StatusCodes.Status400BadRequest,
            title: "VALIDATION_EXCEPTION",
            detail: "Please refer to the errors property for additional details"
        );

        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
