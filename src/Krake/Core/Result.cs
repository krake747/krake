using System.Text;
using Krake.Core.Functional;
using OneOf;

namespace Krake.Core;

public sealed class Result<TError, TValue>(OneOf<TError, TValue> oneOf)
    : OneOfBase<TError, TValue>(oneOf) where TError : IError
{
    private const string Name = nameof(Result<TError, TValue>);
    public bool IsError => IsT0;
    public bool IsValue => IsT1;
    public TError AsError => IsError ? AsT0 : throw new InvalidOperationException($"{Name} is in Value state");
    public TValue AsValue => IsValue ? AsT1 : throw new InvalidOperationException($"{Name} is in Error state");
    public static implicit operator Result<TError, TValue>(TError error) => new(error);
    public static explicit operator TError(Result<TError, TValue> error) => error.AsT0;
    public static implicit operator Result<TError, TValue>(TValue value) => new(value);
    public static explicit operator TValue(Result<TError, TValue> value) => value.AsT1;

    private static Result<TError, TValue> Left(TError error) => error;
    private static Result<TError, TValue> Right(TValue value) => value;

    public TValue AsValueOrDefault(Func<TError, TValue> fallback) =>
        Match(fallback, value => value);

    public Result<TError, TOut> Bind<TOut>(Func<TValue, Result<TError, TOut>> binder) =>
        Match(error => error, binder);

    public Result<TOutError, TValue> BindError<TOutError>(Func<TError, Result<TOutError, TValue>> binder)
        where TOutError : IError =>
        Match(binder, value => value);

    public Result<TOutError, TOut> MapResult<TOutError, TOut>(Func<TError, TOutError> mapError, Func<TValue, TOut> map)
        where TOutError : IError =>
        Match<Result<TOutError, TOut>>(error => mapError(error), value => map(value));

    public Result<TError, TOut> Map<TOut>(Func<TValue, TOut> map) =>
        MapResult(error => error, map);

    public Result<TOutError, TValue> MapError<TOutError>(Func<TError, TOutError> mapError)
        where TOutError : IError =>
        MapResult(mapError, value => value);

    public Result<TError, TValue> Filter(Func<TValue, bool> filter) => Match(
        error => error,
        value => filter(value) ? value : Right(value));

    public Result<TError, TValue> FilterError(Func<TError, bool> filter) => Match(
        error => filter(error) ? error : Left(error),
        value => value);

    public Result<TError, TValue> Do(Action<TValue> action) =>
        IsValue ? AsValue.Tap(action) : this;

    public Result<TError, TValue> Do(Action action) =>
        IsValue ? AsValue.Tap(action) : this;

    public Result<TError, TValue> DoIfError(Action<TError> action) =>
        IsError ? AsError.Tap(action) : this;

    public Result<TError, TValue> DoIfError(Action action) =>
        IsError ? AsError.Tap(action) : this;

    public override string ToString() =>
        new StringBuilder()
            .Append(nameof(Result<TError, TValue>))
            .Append(" { ")
            .Append(IsT0 ? $"{AsT0}" : $"{AsT1}")
            .Append(" }")
            .ToString();
}