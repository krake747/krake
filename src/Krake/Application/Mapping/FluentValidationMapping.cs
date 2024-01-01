using FluentValidation.Results;
using Krake.Core;

namespace Krake.Application.Mapping;

public static class FluentValidationMapping
{
    public static IReadOnlyList<Error> MapToErrors(this IEnumerable<ValidationFailure> failures) =>
        failures.Select(CreateValidationError)
            .ToList()
            .AsReadOnly();

    private static Error CreateValidationError(ValidationFailure failure) =>
        Error.Validation(failure.ErrorMessage)
            .WithPropertyName(failure.PropertyName)
            .WithAttemptedValue(failure.AttemptedValue);
}