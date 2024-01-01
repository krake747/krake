namespace Krake.Contracts.Errors.Responses;

public sealed class ErrorsResponse
{
    public required IEnumerable<ErrorResponse> Errors { get; init; } = Enumerable.Empty<ErrorResponse>();
}