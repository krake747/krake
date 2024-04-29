using System.Text;
using OneOf;

namespace Krake.Core.Results;

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

    public bool TryPickError(out TError error, out TValue value) =>
        TryPickT0(out error, out value);

    public bool TryPickValue(out TValue value, out TError error) =>
        TryPickT1(out value, out error);

    public TValue AsValueOrDefault(Func<TError, TValue> fallback) =>
        Match(fallback, value => value);

    public Result<TError, TOut> Bind<TOut>(Func<TValue, Result<TError, TOut>> binder) =>
        Match(error => error, binder);

    public Result<TError, TOut> Map<TOut>(Func<TValue, TOut> map) =>
        MapResult(error => error, map);

    private Result<TOutError, TOut> MapResult<TOutError, TOut>(Func<TError, TOutError> mapError, Func<TValue, TOut> map)
        where TOutError : IError =>
        Match<Result<TOutError, TOut>>(error => mapError(error), value => map(value));


    public override string ToString() =>
        new StringBuilder()
            .Append(nameof(Result<TError, TValue>))
            .Append(" { ")
            .Append(IsT0 ? $"{AsT0}" : $"{AsT1}")
            .Append(" }")
            .ToString();
}