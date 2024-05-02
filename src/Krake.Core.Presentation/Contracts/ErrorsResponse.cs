namespace Krake.Core.Presentation.Contracts;

public sealed class ErrorsResponse
{
    public required string Message { get; init; }
    public required string Code { get; init; }
    public required ErrorResponseType Type { get; init; }
    public required IEnumerable<ErrorResponse> Items { get; init; } = [];
}