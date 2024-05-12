using System.Text;
using Krake.Core.Functional;
using OneOf;

namespace Krake.Core.Monads;

public sealed class ErrorOr<TValue>(OneOf<ErrorBase, TValue> oneOf)
    : OneOfBase<ErrorBase, TValue>(oneOf), IOneOfErrorOr<TValue>
{
    private const string Name = nameof(ErrorOr<TValue>);
    public bool IsError => IsT0;
    public bool IsValue => IsT1;
    public ErrorBase AsError => IsError ? AsT0 : throw new InvalidOperationException($"{Name} is in Value state");
    public TValue AsValue => IsValue ? AsT1 : throw new InvalidOperationException($"{Name} is in Error state");

    public bool TryPickError(out ErrorBase error, out TValue value) =>
        TryPickT0(out error, out value);

    public bool TryPickValue(out TValue value, out ErrorBase error) =>
        TryPickT1(out value, out error);

    public static implicit operator ErrorOr<TValue>(ErrorBase error) => new(error);
    public static explicit operator ErrorBase(ErrorOr<TValue> error) => error.AsT0;
    public static implicit operator ErrorOr<TValue>(TValue value) => new(value);
    public static explicit operator TValue(ErrorOr<TValue> value) => value.AsT1;

    public TValue AsValueOrDefault(Func<ErrorBase, TValue> fallback) =>
        Match(fallback, value => value);

    public Result<ErrorBase, TOut> Bind<TOut>(Func<TValue, Result<ErrorBase, TOut>> binder) =>
        Match(error => error, binder);

    public Result<ErrorBase, TOut> Map<TOut>(Func<TValue, TOut> map) =>
        MapResult(error => error, map);

    private Result<TOutError, TOut> MapResult<TOutError, TOut>(Func<ErrorBase, TOutError> mapError,
        Func<TValue, TOut> map)
        where TOutError : IError =>
        Match<Result<TOutError, TOut>>(error => mapError(error), value => map(value));

    public ErrorOr<TValue> Do(Action<TValue> action) =>
        IsValue ? AsValue.Tap(action) : this;

    public ErrorOr<TValue> Do(Action action) =>
        IsValue ? AsValue.Tap(action) : this;

    public ErrorOr<TValue> DoIfError(Action<ErrorBase> action) =>
        IsError ? AsError.Tap(action) : this;

    public ErrorOr<TValue> DoIfError(Action action) =>
        IsError ? AsError.Tap(action) : this;

    public override string ToString() =>
        new StringBuilder()
            .Append(Name)
            .Append(" { ")
            .Append(IsT0 ? $"{AsT0}" : $"{AsT1}")
            .Append(" }")
            .ToString();
}