namespace Krake.Core.Builders;

public interface IFluentFunctionalBuilder<out T, out TBuilder>
    where TBuilder : IFluentFunctionalBuilder<T, TBuilder>
    where T : new()
{
    T Build();
    TBuilder Do(Action<T> action);
}