namespace Krake.Contracts.Errors.Responses;

public sealed class ErrorResponse
{
    public required string Message { get; init; }
    public required ErrorResponseType ResponseType { get; init; }
    public string? Property { get; set; }
    public object? AttemptedValue { get; set; }
}