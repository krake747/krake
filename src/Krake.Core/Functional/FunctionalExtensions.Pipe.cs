namespace Krake.Core.Functional;

public static partial class FunctionalExtensions
{
    public static TOut Pipe<TIn, TOut>(this TIn source, Func<TIn, TOut> func) =>
        func(source);

    public static async Task<TOut> PipeAsync<TIn, TOut>(this TIn source, Func<TIn, Task<TOut>> asyncFunc) =>
        await asyncFunc(source);

    public static async Task<TOut> PipeAsync<TIn, TOut>(this TIn source,
        Func<TIn, CancellationToken, Task<TOut>> asyncFunc, CancellationToken token = default) =>
        await asyncFunc(source, token);
}