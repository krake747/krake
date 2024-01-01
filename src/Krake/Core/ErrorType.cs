using System.Text.Json.Serialization;

namespace Krake.Core;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorType
{
    Custom,
    Failure,
    Unexpected,
    Validation,
    Conflict,
    NotFound,
    Unauthorized
}