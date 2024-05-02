namespace Krake.Core.Monads;

public static class Ok
{
    public static Created Created => new();
    public static Updated Updated => new();
    public static Deleted Deleted => new();
    public static Success Success => new();
}

public readonly record struct Created
{
    public override string ToString() => nameof(Created);
}

public readonly record struct Updated
{
    public override string ToString() => nameof(Updated);
}

public readonly record struct Deleted
{
    public override string ToString() => nameof(Deleted);
}

public readonly record struct Success
{
    public override string ToString() => nameof(Success);
}