using Krake.Core.Functional;
using Krake.Core.Monads;
using Krake.Core.Presentation.Contracts;
using Microsoft.AspNetCore.Http;
using static Krake.Core.Monads.ErrorBase;

namespace Krake.Core.Presentation.Mapping;

public static class ApiErrorMapping
{
    public static IResult MapToApiResult(this ErrorBase errorBase)
    {
        return errorBase switch
        {
            Error error => MapToErrorResponse(error).Pipe(e => Results.Problem(
                title: GetTitle(e),
                detail: GetDetail(e),
                statusCode: MapToStatusCode(e.Type),
                type: MapToType(e.Type),
                extensions: MapToExtensions(error))),
            Errors errors => MapToErrorsResponse(errors).Pipe(e => Results.Problem(
                title: e.Code,
                detail: e.Message,
                statusCode: MapToStatusCode(e.Type),
                type: MapToType(e.Type),
                extensions: MapToExtensions(errors))),
            _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    private static ErrorResponse MapToErrorResponse(Error error) => new()
    {
        Message = error.Message,
        Code = error.Code,
        Type = error.Type switch
        {
            ErrorType.Validation => ErrorResponseType.Validation,
            ErrorType.NotFound => ErrorResponseType.NotFound,
            ErrorType.Problem => ErrorResponseType.Problem,
            ErrorType.Conflict => ErrorResponseType.Conflict,
            _ => ErrorResponseType.Unknown
        }
    };

    private static ErrorsResponse MapToErrorsResponse(Errors errors) => new()
    {
        Message = errors.Message,
        Code = errors.Code,
        Type = errors.Type is ErrorType.Collection
            ? ErrorResponseType.Collection
            : ErrorResponseType.Unknown,
        Items = errors.Items.Select(MapToErrorResponse)
    };

    private static string GetDetail(ErrorResponse error) => error.Type switch
    {
        ErrorResponseType.Validation => error.Message,
        ErrorResponseType.NotFound => error.Message,
        ErrorResponseType.Problem => error.Message,
        ErrorResponseType.Conflict => error.Message,
        _ => "An unexpected error occurred"
    };

    private static string GetTitle(ErrorResponse error) => error.Type switch
    {
        ErrorResponseType.Validation => error.Code,
        ErrorResponseType.NotFound => error.Code,
        ErrorResponseType.Problem => error.Code,
        ErrorResponseType.Conflict => error.Code,
        _ => "Server Failure"
    };

    private static int MapToStatusCode(ErrorResponseType type) => type switch
    {
        ErrorResponseType.Collection => StatusCodes.Status400BadRequest,
        ErrorResponseType.Validation => StatusCodes.Status400BadRequest,
        ErrorResponseType.Problem => StatusCodes.Status400BadRequest,
        ErrorResponseType.NotFound => StatusCodes.Status404NotFound,
        ErrorResponseType.Conflict => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };

    private static string MapToType(ErrorResponseType type) => type switch
    {
        ErrorResponseType.Collection => "https://datatracker.ietf.org/html/rfc7231#section-6.5.1",
        ErrorResponseType.Validation => "https://datatracker.ietf.org/html/rfc7231#section-6.5.1",
        ErrorResponseType.NotFound => "https://datatracker.ietf.org/html/rfc7231#section-6.5.4",
        ErrorResponseType.Problem => "https://datatracker.ietf.org/html/rfc7231#section-6.5.1",
        ErrorResponseType.Conflict => "https://datatracker.ietf.org/html/rfc7231#section-6.5.8",
        _ => "https://datatracker.ietf.org/html/rfc7231#section-6.6.1"
    };

    private static Dictionary<string, object?>? MapToExtensions(ErrorBase errorBase)
    {
        var extensions = new Dictionary<string, object?>();
        var added = errorBase switch
        {
            Errors errors =>
                extensions.TryAdd("items", errors.Items),
            Error error and ({ PropertyName: not null } or { AttemptedValue: not null }) =>
                extensions.TryAdd("error", new { error.PropertyName, error.AttemptedValue }),
            _ => false
        };

        return added ? extensions : null;
    }
}