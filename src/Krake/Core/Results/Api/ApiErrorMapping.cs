// namespace Krake.Core.Results.Api;
//
// public static class ApiErrorMapping
// {
//     public static IResult MapToApiResult(this ErrorBase errorBase)
//     {
//         var errorResponse = MapToErrorResponse(errorBase):
//         return Results.Problem(
//             detail: MapToDetail(errorResponse),
//             statusCode: MapToStatusCode(errorResponse),
//             title: MapToTitle(errorResponse),
//             type: MapToType(errorResponse),
//             extensions: MapToExtensions(errorBase));
//     }
//
//     private static ErrorResponse MapToErrorResponse(ErrorBase errorBase) => new()
//     {
//         Message = errorBase.Message,
//         Code = errorBase.Code,
//         Type = errorBase.Type switch
//         {
//             ErrorType.Validation => ErrorResponseType.Validation,
//             ErrorType.NotFound => ErrorResponseType.NotFound,
//             ErrorType.Problem => ErrorResponseType.Problem,
//             ErrorType.Conflict => ErrorResponseType.Conflict,
//             _ => ErrorResponseType.Unknown
//         },
//         PropertyName = errorBase.PropertyName,
//         AttemptedValue = errorBase.AttemptedValue
//     };
//
//     private static string MapToDetail(ErrorResponse error) => error.Type switch
//     {
//         ErrorResponseType.Validation => error.Message,
//         ErrorResponseType.NotFound => error.Message,
//         ErrorResponseType.Problem => error.Message,
//         ErrorResponseType.Conflict => error.Message,
//         _ => "An unexpected error occurred"
//     };
//
//     private static int MapToStatusCode(ErrorResponse error) => error.Type switch
//     {
//         ErrorResponseType.Validation => StatusCodes.Status400BadRequest,
//         ErrorResponseType.Problem => StatusCodes.Status400BadRequest,
//         ErrorResponseType.NotFound => StatusCodes.Status404NotFound,
//         ErrorResponseType.Conflict => StatusCodes.Status409Conflict,
//         _ => StatusCodes.Status500InternalServerError
//     };
//
//     private static string MapToTitle(ErrorResponse error) => error.Type switch
//     {
//         ErrorResponseType.Validation => error.Code,
//         ErrorResponseType.NotFound => error.Code,
//         ErrorResponseType.Problem => error.Code,
//         ErrorResponseType.Conflict => error.Code,
//         _ => "Server Failure"
//     };
//
//     private static string MapToType(ErrorResponse error) => error.Type switch
//     {
//         ErrorResponseType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
//         ErrorResponseType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
//         ErrorResponseType.Problem => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
//         ErrorResponseType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
//         _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
//     };
//
//     private static Dictionary<string, object?>? MapToExtensions(ErrorBase errorBase)
//     {
//         var extensions = new Dictionary<string, object?>();
//         var added = errorBase switch
//         {
//             ValidationErrors validationErrors =>
//                 extensions.TryAdd("errors", validationErrors.Items.Select(MapToErrorResponse)),
//             Errors errors =>
//                 extensions.TryAdd("errors", errors.Items.Select(MapToErrorResponse)),
//             Error error and ({ PropertyName: not null } or { AttemptedValue: not null }) =>
//                 extensions.TryAdd("errors", new { error.PropertyName, error.AttemptedValue }),
//             _ => false
//         };
//
//         return added ? extensions : null;
//     }
// }