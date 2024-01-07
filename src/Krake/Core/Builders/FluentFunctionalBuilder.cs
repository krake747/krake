namespace Krake.Core.Builders;

public abstract class FluentFluentFunctionalBuilder<T, TBuilder> : IFluentFunctionalBuilder<T, TBuilder>
    where TBuilder : FluentFluentFunctionalBuilder<T, TBuilder>
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