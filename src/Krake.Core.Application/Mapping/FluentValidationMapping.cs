using FluentValidation.Results;
using static Krake.Core.Monads.ErrorBase;

namespace Krake.Core.Application.Mapping;

public static class FluentValidationMapping
{
    public static Errors MapToErrors(this IEnumerable<ValidationFailure> failures) =>
        Errors.FromCollection(failures.Select(CreateValidationError));

    private static Error CreateValidationError(ValidationFailure failure) =>
        Error.Validation(
            failure.ErrorMessage,
            $"FluentValidation.{failure.ErrorCode}",
            failure.PropertyName,
            failure.AttemptedValue);
}