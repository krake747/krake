using System.Diagnostics;
using System.Globalization;
using System.Text;
using Krake.Core.Functional;

namespace Krake.Core.Monads;

public abstract record ErrorBase : IError
{
    private static readonly CultureInfo CultureInfo = CultureInfo.InvariantCulture;

    private ErrorBase(string message, string code, ErrorType type)
    {
        Message = message;
        Code = code;
        Type = type;
    }

    public string Code { get; private init; }
    public string Message { get; private init; }
    public ErrorType Type { get; private init; }


    [DebuggerDisplay("Message = {Message}, Type = {Type}, Code = {Code}")]
    public sealed record Error : ErrorBase
    {
        private const string Prefix = "General";

        private Error(string message, string code, ErrorType type)
            : base(message, code, type)
        {
        }

        public string? PropertyName { get; private set; }
        public object? AttemptedValue { get; private set; }

        public static Error Validation(
            string message = "A validation error has occurred.",
            string code = $"{Prefix}.{nameof(Validation)}",
            string? propertyName = null,
            object? attemptedValue = null) =>
            new(message, code, ErrorType.Validation) { PropertyName = propertyName, AttemptedValue = attemptedValue };

        public static Error NotFound(
            string message = "A 'Not Found' error has occurred.",
            string code = $"{Prefix}.{nameof(NotFound)}") =>
            new(message, code, ErrorType.NotFound);

        public static Error Problem(
            string message = "A problem error has occurred.",
            string code = $"{Prefix}.{nameof(Problem)}") =>
            new(message, code, ErrorType.Problem);

        public static Error Failure(
            string message = "A failure has occurred.",
            string code = $"{Prefix}.{nameof(Failure)}") =>
            new(message, code, ErrorType.Failure);

        public static Error Conflict(
            string message = "A conflict error has occurred.",
            string code = $"{Prefix}.{nameof(Conflict)}") =>
            new(message, code, ErrorType.Conflict);

        public static Error Unexpected(
            string message = "An unexpected error has occurred.",
            string code = $"{Prefix}.{nameof(Unexpected)}") =>
            new(message, code, ErrorType.Unexpected);

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

        private bool PrintOptionalMembers(StringBuilder sb)
        {
            sb.Append(string.Create(CultureInfo, $"Message = {Message}, Type = {Type}, Code = {Code}"));

            if (PropertyName is not null)
            {
                sb.Append(string.Create(CultureInfo, $", PropertyName = {PropertyName}"));
            }

            if (AttemptedValue is not null)
            {
                sb.Append(string.Create(CultureInfo, $", AttemptedValue = {AttemptedValue}"));
            }

            return true;
        }
    }

    [DebuggerDisplay("Errors Count = {Count}")]
    public sealed record Errors : ErrorBase
    {
        private readonly List<Error> _items;

        private Errors(IEnumerable<Error> items)
            : base("One or more errors occurred", $"{nameof(Errors)}.{nameof(ErrorType.Collection)}",
                ErrorType.Collection)
        {
            _items = items.ToList();
        }

        public IEnumerable<Error> Items => _items.AsReadOnly();
        public int Count => _items.Count;

        public static Errors FromError(Error error) =>
            new([error]);

        public static Errors FromCollection(IEnumerable<Error> errors) =>
            new(errors);

        public static Errors FromResults<TValue>(IEnumerable<Result<Error, TValue>> results) =>
            new(results.Where(r => r.IsError).Select(r => r.AsError));

        public void Add(Error error) => _items.Add(error);
        public void AddRange(IEnumerable<Error> errors) => _items.AddRange(errors);

        public override string ToString() =>
            new StringBuilder()
                .Append(nameof(Errors))
                .Append(" { ")
                .Append(string.Create(CultureInfo, $"Count = {Count}"))
                .Append(" }")
                .ToString();
    }
}