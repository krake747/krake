namespace Krake.Core.Builders;

public abstract class FluentFunctionalBuilder<T, TBuilder> : IFluentFunctionalBuilder<T, TBuilder>
    where TBuilder : FluentFunctionalBuilder<T, TBuilder>
    where T : new()
{
    private readonly List<Func<T, T>> _actions = [];

    public T Build() => _actions.Aggregate(new T(), (x, f) => f(x));

    public TBuilder Do(Action<T> action)
    {
        _actions.Add(x =>
        {
            action(x);
            return x;
        });

        return (TBuilder)this;
    }
}