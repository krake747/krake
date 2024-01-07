using OneOf;

namespace Krake.Core.Builders;

public interface IResultFunctionalBuilder<out TErrorOrT, out T, out TBuilder>
    where TErrorOrT : OneOfBase<Errors, T>
    where TBuilder : IResultFunctionalBuilder<TErrorOrT, T, TBuilder>
{
    TErrorOrT Build();
    TBuilder Do(Action<T> action);
}