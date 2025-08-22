using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SmartTestTask.Domain.Exceptions;

namespace SmartTestTask.API.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";
        
        var problemDetails = CreateProblemDetails(context, exception);
        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        };
        
        var json = JsonSerializer.Serialize(problemDetails, options);
        await context.Response.WriteAsync(json);
    }

    private ProblemDetails CreateProblemDetails(HttpContext context, Exception exception)
    {
        var (statusCode, title, detail, type) = exception switch
        {
            InsufficientAreaException insufficientArea => (
                StatusCodes.Status409Conflict,
                "Insufficient Area",
                insufficientArea.Message,
                "https://smarttesttask.com/errors/insufficient-area"
            ),
            
            EntityNotFoundException notFound => (
                StatusCodes.Status404NotFound,
                "Entity Not Found",
                notFound.Message,
                "https://smarttesttask.com/errors/not-found"
            ),
            
            BusinessRuleValidationException businessRule => (
                StatusCodes.Status400BadRequest,
                "Business Rule Violation",
                businessRule.Message,
                "https://smarttesttask.com/errors/business-rule"
            ),
            
            ContractValidationException validation => (
                StatusCodes.Status400BadRequest,
                "Validation Error",
                validation.Message,
                "https://smarttesttask.com/errors/validation"
            ),
            
            ArgumentException argException => (
                StatusCodes.Status400BadRequest,
                "Invalid Argument",
                argException.Message,
                "https://smarttesttask.com/errors/invalid-argument"
            ),
            
            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                _environment.IsDevelopment() ? exception.Message : "An error occurred while processing your request",
                "https://smarttesttask.com/errors/internal"
            )
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = type,
            Instance = context.Request.Path
        };

        // Add request ID for tracing
        problemDetails.Extensions["requestId"] = context.TraceIdentifier;
        problemDetails.Extensions["timestamp"] = DateTimeOffset.UtcNow;

        // Add additional data for specific exceptions
        if (exception is InsufficientAreaException insufficientAreaEx)
        {
            problemDetails.Extensions["facilityCode"] = insufficientAreaEx.FacilityCode;
            problemDetails.Extensions["requiredArea"] = insufficientAreaEx.RequiredArea;
            problemDetails.Extensions["availableArea"] = insufficientAreaEx.AvailableArea;
        }
        else if (exception is EntityNotFoundException entityNotFoundEx)
        {
            problemDetails.Extensions["entityType"] = entityNotFoundEx.EntityType;
            problemDetails.Extensions["entityId"] = entityNotFoundEx.EntityId;
        }
        else if (exception is BusinessRuleValidationException businessRuleEx)
        {
            problemDetails.Extensions["ruleName"] = businessRuleEx.RuleName;
        }

        // In development, add stack trace
        if (_environment.IsDevelopment() && exception.StackTrace != null)
        {
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        return problemDetails;
    }
}