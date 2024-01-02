using System.Diagnostics;
using System.Text;

namespace Krake.Core;

[DebuggerDisplay("Message = {Message}, Type = {Type}")]
public readonly record struct Error : IError
{
    private Error(string message, ErrorType type)
    {
        Message = message;
        Type = type;
    }

    public string Message { get; }
    public ErrorType Type { get; }
    public string? PropertyName { get; internal init; }
    public object? AttemptedValue { get; internal init; }

    public static Error Custom(string message = "A custom error has occurred.") =>
        new(message, ErrorType.Custom);

    public static Error Failure(string message = "A failure has occurred.") =>
        new(message, ErrorType.Failure);

    public static Error Unexpected(string message = "An unexpected error has occurred.") =>
        new(message, ErrorType.Unexpected);

    public static Error Validation(string message = "A validation error has occurred.") =>
        new(message, ErrorType.Validation);

    public static Error Conflict(string message = "A conflict error has occurred.") =>
        new(message, ErrorType.Conflict);

    public static Error NotFound(string message = "A 'Not Found' error has occurred.") =>
        new(message, ErrorType.NotFound);

    public static Error Unauthorized(string message = "An unauthorized error has occurred.") =>
        new(message, ErrorType.Unauthorized);

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(nameof(Error));
        sb.Append(" { ");
        if (PrintMembers(sb))
        {
            sb.Append(' ');
        }

        sb.Append('}');

        return sb.ToString();
    }

    private bool PrintMembers(StringBuilder sb)
    {
        sb.Append($"Message = {Message}, Type = {Type}");

        if (PropertyName is not null)
        {
            sb.Append($", PropertyName = {PropertyName}");
        }

        if (AttemptedValue is not null)
        {
            sb.Append($", AttemptedValue = {AttemptedValue}");
        }

        return true;
    }
}

public static class ErrorBuilder
{
    public static Error WithPropertyName(this Error error, string propertyName) =>
        error with { PropertyName = propertyName };

    public static Error WithAttemptedValue<T>(this Error error, T attemptedValue) =>
        error with { AttemptedValue = attemptedValue };
}