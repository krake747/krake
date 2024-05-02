namespace Krake.Core.Monads;

public interface IOneOfResult<TError, TValue>
{
    bool IsError { get; }
    bool IsValue { get; }
    TError AsError { get; }
    TValue AsValue { get; }
    bool TryPickError(out TError error, out TValue value);
    bool TryPickValue(out TValue value, out TError error);
}