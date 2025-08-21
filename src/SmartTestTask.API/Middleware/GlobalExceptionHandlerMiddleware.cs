using System.Text.Json;
using SmartTestTask.Application.DTOs.Responce;
using SmartTestTask.Domain.Exceptions;

namespace SmartTestTask.API.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ApiResponse<object>
        {
            Success = false,
            Data = null,
            Errors = new List<string>()
        };

        switch (exception)
        {
            case InsufficientAreaException insufficientArea:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = insufficientArea.Message;
                response.Errors.Add($"Required area: {insufficientArea.RequiredArea} m²");
                response.Errors.Add($"Available area: {insufficientArea.AvailableArea} m²");
                break;
                
            case EntityNotFoundException notFound:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = notFound.Message;
                break;
                
            case BusinessRuleValidationException businessRule:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = businessRule.Message;
                response.Errors.Add($"Rule: {businessRule.RuleName}");
                break;
                
            case ContractValidationException validation:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = validation.Message;
                break;
                
            case ArgumentException argException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = argException.Message;
                break;
                
            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while processing your request";
                response.Errors.Add(exception.Message);
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}
