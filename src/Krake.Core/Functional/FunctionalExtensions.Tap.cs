namespace Krake.Core.Functional;

public static partial class FunctionalExtensions
{
    public static T Tap<T>(this T source, Action<T> action)
    {
        action(source);
        return source;
    }

    public static T Tap<T>(this T source, Action action)
    {
        action();
        return source;
    }

    public static async Task<T> TapAsync<T>(this Task<T> sourceTask, Action<T> action)
    {
        var source = await sourceTask;
        action(source);
        return source;
    }

    public static async Task<T> TapAsync<T>(this Task<T> sourceTask, Action action)
    {
        var source = await sourceTask;
        action();
        return source;
    }

    public static async Task<T> TapAsync<T>(this Task<T> sourceTask, Func<T, Task> asyncAction)
    {
        var source = await sourceTask;
        await asyncAction(source);
        return source;
    }
}