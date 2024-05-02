namespace Krake.Core.Presentation.Contracts;

public sealed class ErrorResponse
{
    public required string Message { get; init; }
    public required string Code { get; init; }
    public required ErrorResponseType Type { get; init; }
    public string? PropertyName { get; set; }
    public object? AttemptedValue { get; set; }
}