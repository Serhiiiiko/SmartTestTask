using FluentValidation;
using MediatR;
using SmartTestTask.Domain.Errors;
using SmartTestTask.Domain.Results;

namespace SmartTestTask.Application.PipelineBehaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        
        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure != null)
            .ToList();

        if (failures.Any())
        {
            var firstError = failures.First();
            var error = DomainErrors.Validation.InvalidInput(firstError.PropertyName);
            
            // Check if TResponse is Result<T> type
            var responseType = typeof(TResponse);
            if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var resultType = responseType.GetGenericArguments()[0];
                var failureMethod = typeof(Result<>)
                    .MakeGenericType(resultType)
                    .GetMethod("Failure", new[] { typeof(Error) });
                
                return (TResponse)failureMethod.Invoke(null, new object[] { error });
            }
            
            // If it's just Result
            if (responseType == typeof(Result))
            {
                return (TResponse)(object)Result.Failure(error);
            }
        }

        return await next();
    }
}