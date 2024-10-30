using EmployeeManagement.Domain.Abstractions;
using FluentValidation;
using MediatR;

namespace EmployeeManagement.Application.Abstractions.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationErrors = _validators
            .Select(validator => validator.Validate(context))
            .Where(result => !result.IsValid)
            .SelectMany(result => result.Errors)
            .Select(failure => new Error(failure.PropertyName, failure.ErrorMessage))
            .ToList();

        if (validationErrors.Count > 0)
        {
            return (TResponse) Result.Failure(validationErrors);
        }

        return await next();
    }
}
