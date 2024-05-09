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
    public string Country { get; private set; } = string.Empty;
    public string Mic { get; private set; } = string.Empty;
    public string Sector { get; private set; } = string.Empty;
    public string Symbol { get; private set; } = string.Empty;
    public string Isin { get; private set; } = string.Empty;

    public static Instrument From(string name, string currency, string country, string mic, string sector,
        string symbol, string isin) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        Currency = currency,
        Country = country,
        Mic = mic,
        Sector = sector,
        Symbol = symbol,
        Isin = isin,
    };
}