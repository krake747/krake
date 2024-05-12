namespace Krake.Core.Monads;

public interface IOneOfErrorOr<TValue> : IOneOfResult<ErrorBase, TValue>
{
}