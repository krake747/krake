using System.Text.Json.Serialization;

namespace Krake.Contracts.Errors.Responses;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorResponseType
{
    Unknown,
    Failure,
    Unexpected,
    Validation,
    Conflict,
    NotFound,
    Unauthorized
}