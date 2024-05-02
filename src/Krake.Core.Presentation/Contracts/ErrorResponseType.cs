using System.Text.Json.Serialization;

namespace Krake.Core.Presentation.Contracts;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorResponseType
{
    Unknown,
    Collection,
    Validation,
    NotFound,
    Problem,
    Conflict
}