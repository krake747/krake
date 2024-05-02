using System.Text.Json.Serialization;

namespace Krake.Core.Monads;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorType
{
    Unknown,
    Collection,
    Validation,
    NotFound,
    Problem,
    Failure,
    Conflict,
    Unexpected
}