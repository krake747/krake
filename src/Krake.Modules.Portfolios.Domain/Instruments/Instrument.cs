using Krake.Core.Domain;
using Krake.Core.Monads;

namespace Krake.Modules.Portfolios.Domain.Instruments;

public sealed class Instrument : Entity
{
    private readonly List<InstrumentPrice> _prices = [];

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
    public IReadOnlyList<InstrumentPrice> Prices => _prices.AsReadOnly();

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
        Isin = isin
    };

    public Success AddInvestment(InstrumentPrice instrumentPrice)
    {
        _prices.Add(instrumentPrice);
        return Ok.Success;
    }
}