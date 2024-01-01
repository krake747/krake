namespace Krake.Core;

public readonly record struct Errors(IReadOnlyList<Error> Items) : IError;