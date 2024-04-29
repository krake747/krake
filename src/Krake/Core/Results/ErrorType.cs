using System.Text.Json.Serialization;

namespace Krake.Core.Results;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorType
{
    Custom,
    Validation,
    NotFound,
    Problem,
    Failure,
    Conflict,
    Unexpected
}