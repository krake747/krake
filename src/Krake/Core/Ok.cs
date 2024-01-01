namespace Krake.Core;

public static class Ok
{
    public static Created Created => new();
    public static Updated Updated => new();
    public static Deleted Deleted => new();
    public static Success Success => new();
}

public readonly struct Created
{
    public override string ToString() => nameof(Created);
}

public readonly struct Updated
{
    public override string ToString() => nameof(Updated);
}

public readonly struct Deleted
{
    public override string ToString() => nameof(Deleted);
}

public readonly struct Success
{
    public override string ToString() => nameof(Success);
}