using Krake.Contracts.Errors.Responses;
using Krake.Core;

namespace Krake.Api.Mapping;

public static class ErrorMapping
{
    public static ErrorsResponse MapToResponse(this Errors errors) => new()
    {
        Errors = errors.Items.Select(MapToResponse)
    };

    public static ErrorResponse MapToResponse(this Error error) => new()
    {
        Message = error.Message,
        ResponseType = MapToErrorResponseType(error.Type),
        Property = error.PropertyName,
        AttemptedValue = error.AttemptedValue
    };

    private static ErrorResponseType MapToErrorResponseType(ErrorType type) => type switch
    {
        ErrorType.Failure => ErrorResponseType.Failure,
        ErrorType.Unexpected => ErrorResponseType.Unexpected,
        ErrorType.Validation => ErrorResponseType.Validation,
        ErrorType.Conflict => ErrorResponseType.Conflict,
        ErrorType.NotFound => ErrorResponseType.NotFound,
        ErrorType.Unauthorized => ErrorResponseType.Unauthorized,
        _ => ErrorResponseType.Unknown
    };
}