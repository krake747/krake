using Krake.Core.Domain;

namespace Krake.Modules.Portfolios.Domain.Instruments;

public sealed class Instrument : Entity
{
    private Instrument()
    {
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Currency { get; private set; } = string.Empty;

    public static Instrument From(string name, string currency) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        Currency = currency
    };
}