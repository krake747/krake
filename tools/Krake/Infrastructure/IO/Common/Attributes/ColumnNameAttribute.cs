namespace Krake.Infrastructure.IO.Common.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ColumnNameAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}