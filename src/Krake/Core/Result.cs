using System.Text;
using OneOf;

namespace Krake.Core;

public sealed class Result<TError, TValue>(OneOf<TError, TValue> oneOf) : OneOfBase<TError, TValue>(oneOf)
    where TError : IError
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

    public Result<TError, TOut> Bind<TOut>(Func<TValue, Result<TError, TOut>> binder) => Match(
        error => error,
        binder);

    public Result<TOut, TValue> BindError<TOut>(Func<TError, Result<TOut, TValue>> binder)
        where TOut : IError => Match(
        binder,
        value => value);
    
    public Result<TError, TOut> Map<TOut>(Func<TValue, TOut> map) => Match<Result<TError, TOut>>(
        error => error,
        value => map(value));
    
    public Result<TOut, TValue> MapError<TOut>(Func<TError, TOut> mapError)
        where TOut : IError => Match<Result<TOut, TValue>>(
        error => mapError(error),
        value => value);
    
    public Result<TOutError, TOut> BiMap<TOutError ,TOut>(Func<TError, TOutError> mapError, Func<TValue, TOut> map)
        where TOutError : IError => Match<Result<TOutError, TOut>>(
        error => mapError(error),
        value => map(value));
    
    public override string ToString() => OneOfBaseToString(this, nameof(Result<TError, TValue>));
    
    private static string OneOfBaseToString<T1, T2>(OneOfBase<T1, T2> oneOfBase, string name) => 
        new StringBuilder()
            .Append(name)
            .Append(" { ")
            .Append(oneOfBase.IsT0 ? $"{oneOfBase.AsT0}" : $"{oneOfBase.AsT1}")
            .Append(" }")
            .ToString();
}