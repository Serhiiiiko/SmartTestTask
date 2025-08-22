using Microsoft.AspNetCore.Mvc;
using SmartTestTask.Domain.Results;

namespace SmartTestTask.API.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        return result.Match<IActionResult>(
            onSuccess: value => new OkObjectResult(value),
            onFailure: error => ToProblemDetails(error));
    }

    public static IActionResult ToActionResult(this Result result)
    {
        return result.Match<IActionResult>(
            onSuccess: () => new NoContentResult(),
            onFailure: error => ToProblemDetails(error));
    }

    private static IActionResult ToProblemDetails(Error error)
    {
        var statusCode = GetStatusCode(error.Code);
        
        return new ObjectResult(new ProblemDetails
        {
            Status = statusCode,
            Title = GetTitle(error.Code),
            Detail = error.Message,
            Type = $"https://httpstatuses.com/{statusCode}"
        })
        {
            StatusCode = statusCode
        };
    }

    private static int GetStatusCode(string errorCode)
    {
        return errorCode switch
        {
            var code when code.Contains("NotFound") => StatusCodes.Status404NotFound,
            var code when code.Contains("Validation") || code.Contains("Invalid") => StatusCodes.Status400BadRequest,
            var code when code.Contains("InsufficientArea") || code.Contains("AlreadyDeactivated") => StatusCodes.Status409Conflict,
            var code when code.Contains("Unauthorized") => StatusCodes.Status401Unauthorized,
            var code when code.Contains("Forbidden") => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    private static string GetTitle(string errorCode)
    {
        return errorCode switch
        {
            var code when code.Contains("NotFound") => "Resource Not Found",
            var code when code.Contains("Validation") || code.Contains("Invalid") => "Validation Error",
            var code when code.Contains("InsufficientArea") => "Business Rule Violation",
            var code when code.Contains("AlreadyDeactivated") => "Operation Not Allowed",
            var code when code.Contains("Unauthorized") => "Unauthorized Access",
            var code when code.Contains("Forbidden") => "Forbidden",
            _ => "Internal Server Error"
        };
    }
}