using System.Text;
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

    public Result<TError, TOut> MapAsync<TOut>(Func<TValue, TOut> map) =>
        MapResult(error => error, map);

    public Result<TOutError, TValue> MapError<TOutError>(Func<TError, TOutError> mapError)
        where TOutError : IError =>
        MapResult(mapError, value => value);

    public override string ToString() =>
        new StringBuilder()
            .Append(nameof(Result<TError, TValue>))
            .Append(" { ")
            .Append(IsT0 ? $"{AsT0}" : $"{AsT1}")
            .Append(" }")
            .ToString();
}