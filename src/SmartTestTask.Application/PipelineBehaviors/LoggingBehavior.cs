using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace SmartTestTask.Application.PipelineBehaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid();
        
        _logger.LogInformation(
            "Handling {RequestName} ({RequestId}) {@Request}",
            requestName,
            requestId,
            request);

        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var response = await next();
            
            stopwatch.Stop();
            
            if (stopwatch.ElapsedMilliseconds > 500)
            {
                _logger.LogWarning(
                    "Long Running Request: {RequestName} ({RequestId}) took {ElapsedMilliseconds}ms",
                    requestName,
                    requestId,
                    stopwatch.ElapsedMilliseconds);
            }
            
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            _logger.LogError(
                ex,
                "Request {RequestName} ({RequestId}) failed after {ElapsedMilliseconds}ms",
                requestName,
                requestId,
                stopwatch.ElapsedMilliseconds);
            
            throw;
        }
    }
}