using System.Diagnostics;
using System.Globalization;
using System.Text;
using FluentValidation.Results;
using Krake.Core.Functional;

namespace Krake.Core.Results;

public abstract record ErrorBase : IError
{
    private const string Prefix = "General";

    private ErrorBase(string message, string code, ErrorType type)
    {
        Message = message;
        Code = code;
        Type = type;
    }

    public string? PropertyName { get; private set; }
    public object? AttemptedValue { get; private set; }
    public string Code { get; }
    public string Message { get; private init; }
    public ErrorType Type { get; private init; }

    private bool PrintOptionalMembers(StringBuilder sb)
    {
        sb.Append($"Message = {Message}, Type = {Type}, Code = {Code}");

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

    [DebuggerDisplay("Message = {Message}, Type = {Type}, Code = {Code}")]
    public sealed record Error(string Message, string Code, ErrorType Type)
        : ErrorBase(Message, Code, Type)
    {
        public static Error Custom(
            string message = "A custom error has occurred.",
            string code = $"{Prefix}.{nameof(Custom)}") =>
            new(code, message, ErrorType.Custom);

        public static Error Validation(
            string message = "A validation error has occurred.",
            string code = $"{Prefix}.{nameof(Validation)}",
            string? propertyName = null,
            object? attemptedValue = null) =>
            new(code, message, ErrorType.Validation) { PropertyName = propertyName, AttemptedValue = attemptedValue };

        public static Error NotFound(
            string message = "A 'Not Found' error has occurred.",
            string code = $"{Prefix}.{nameof(NotFound)}") =>
            new(code, message, ErrorType.NotFound);

        public static Error Problem(
            string message = "A problem error has occurred.",
            string code = $"{Prefix}.{nameof(Problem)}") =>
            new(code, message, ErrorType.Problem);

        public static Error Failure(
            string message = "A failure has occurred.",
            string code = $"{Prefix}.{nameof(Failure)}") =>
            new(code, message, ErrorType.Failure);

        public static Error Conflict(
            string message = "A conflict error has occurred.",
            string code = $"{Prefix}.{nameof(Conflict)}") =>
            new(code, message, ErrorType.Conflict);

        public static Error Unexpected(
            string message = "An unexpected error has occurred.",
            string code = $"{Prefix}.{nameof(Unexpected)}") =>
            new(code, message, ErrorType.Unexpected);

        public Error WithPropertyName(string propertyName) =>
            this.Tap(e => e.PropertyName = propertyName);

        public Error WithAttemptedValue<T>(T attemptedValue) =>
            this.Tap(e => e.AttemptedValue = attemptedValue);

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(nameof(Error));
            sb.Append(" { ");
            if (PrintOptionalMembers(sb))
            {
                sb.Append(' ');
            }

            sb.Append('}');

            return sb.ToString();
        }
    }

    [DebuggerDisplay("Errors Count = {Count}")]
    public sealed record Errors(IEnumerable<Error> Items)
        : ErrorBase("One or more custom errors occurred", $"{Prefix}.Custom", ErrorType.Custom)
    {
        private readonly List<Error> _items = Items.ToList();
        public IEnumerable<Error> Items => _items.AsReadOnly();
        public int Count => _items.Count;
        public void Add(Error error) => _items.Add(error);
        public void AddRange(IEnumerable<Error> errors) => _items.AddRange(errors);

        public static Errors FromErrors(IEnumerable<Error> errors) =>
            new(errors);

        public static Errors FromResults<TValue>(IEnumerable<Result<Error, TValue>> results) =>
            new(results.Where(r => r.IsError).Select(r => r.AsError));

        public override string ToString() =>
            new StringBuilder()
                .Append(nameof(Errors))
                .Append(" { ")
                .Append(string.Create(CultureInfo.InvariantCulture, $"Count = {Count}"))
                .Append(" }")
                .ToString();
    }

    [DebuggerDisplay("ValidationErrors Count = {Count}")]
    public sealed record ValidationErrors(IEnumerable<Error> Items)
        : ErrorBase("One or more validation errors occurred", $"{Prefix}.Validation", ErrorType.Validation)
    {
        private readonly List<Error> _items = Items.ToList();
        public IEnumerable<Error> Items => _items.AsReadOnly();
        public int Count => _items.Count;
        public void Add(Error error) => _items.Add(error);
        public void AddRange(IEnumerable<Error> errors) => _items.AddRange(errors);

        public static ValidationErrors FromErrors(IEnumerable<Error> errors) =>
            new(errors);

        public static ValidationErrors FromResults<TValue>(IEnumerable<Result<Error, TValue>> results) =>
            new(results.Where(r => r is { IsError: true, AsError.Type: ErrorType.Validation }).Select(r => r.AsError));

        public override string ToString() =>
            new StringBuilder()
                .Append(nameof(ValidationErrors))
                .Append(" { ")
                .Append(string.Create(CultureInfo.InvariantCulture, $"Count = {Count}"))
                .Append(" }")
                .ToString();
    }

    public static class FluentValidationMapping
    {
        public static Errors MapToErrors(IEnumerable<ValidationFailure> failures) =>
            Errors.FromErrors(failures.Select(CreateValidationError));

        private static Error CreateValidationError(ValidationFailure failure) =>
            Error.Validation(
                failure.ErrorMessage, "FluentValidation.Validation", failure.PropertyName, failure.AttemptedValue);
    }
}