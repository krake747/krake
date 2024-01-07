using System.Diagnostics;
using System.Text;

namespace Krake.Core;

[DebuggerDisplay("Errors Count = {Count}")]
public sealed class Errors(IEnumerable<Error> items) : IError
{
    private readonly List<Error> _items = items.ToList();
    public IReadOnlyList<Error> Items => _items.AsReadOnly();
    public int Count => _items.Count;

    public void Add(Error error) => _items.Add(error);
    public void AddRange(IEnumerable<Error> errors) => _items.AddRange(errors);

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(nameof(Errors));
        sb.Append(" { ");
        sb.Append($"Count = {Count}");
        sb.Append(" }");
        return sb.ToString();
    }
}