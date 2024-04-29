using System.Text.Json.Serialization;

namespace Krake.Core.Results.Api;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorResponseType
{
    Unknown,
    Validation,
    NotFound,
    Problem,
    Failure,
    Conflict,
    Unexpected
}