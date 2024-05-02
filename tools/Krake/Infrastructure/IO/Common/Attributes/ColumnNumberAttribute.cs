namespace Krake.Infrastructure.IO.Common.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ColumnNumberAttribute(int value) : Attribute
{
    public int Value => value;
}