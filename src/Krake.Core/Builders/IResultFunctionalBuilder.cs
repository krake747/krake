using Krake.Core.Monads;
using OneOf;

namespace Krake.Core.Builders;

public interface IResultFunctionalBuilder<out TResult, out TValue, out TBuilder>
    where TResult : OneOfBase<ErrorBase, TValue>
    where TBuilder : IResultFunctionalBuilder<TResult, TValue, TBuilder>
{
    TResult Build();
    TBuilder Do(Action<TValue> action);
}