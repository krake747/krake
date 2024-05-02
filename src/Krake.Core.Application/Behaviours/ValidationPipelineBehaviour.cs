using FluentValidation;
using FluentValidation.Results;
using Krake.Core.Application.Mapping;
using Krake.Core.Application.Messaging;
using MediatR;
using OneOf;

namespace Krake.Core.Application.Behaviours;

internal sealed class ValidationPipelineBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
    where TResponse : IOneOf
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken token = default)
    {
        var validationFailures = await ValidateAsync([..validators], request, token).ConfigureAwait(false);
        if (validationFailures is [])
        {
            return await next().ConfigureAwait(false);
        }

        if (typeof(TResponse).IsAssignableTo(typeof(IOneOf)))
        {
            return (TResponse)(dynamic)validationFailures.MapToErrors();
        }

        throw new ValidationException(validationFailures);
    }

    private static async Task<ValidationFailure[]> ValidateAsync(IValidator<TRequest>[] validators,
        TRequest request, CancellationToken token = default)
    {
        if (validators is [])
        {
            return [];
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults =
            await Task.WhenAll(validators.Select(validator => validator.ValidateAsync(context, token)));

        var validationFailures = validationResults
            .Where(validationResult => validationResult.IsValid is false)
            .SelectMany(validationResult => validationResult.Errors)
            .ToArray();

        return validationFailures;
    }
}