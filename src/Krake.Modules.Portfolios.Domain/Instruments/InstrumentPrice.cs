namespace Krake.Modules.Portfolios.Domain.Instruments;

public sealed record InstrumentPrice
{
    private InstrumentPrice()
    {
    }

    public DateOnly Date { get; private set; }
    public decimal Open { get; private set; }
    public decimal High { get; private set; }
    public decimal Low { get; private set; }
    public decimal Close { get; private set; }
    public decimal AdjustedClose { get; private set; }
    public decimal Volume { get; private set; }

    public static InstrumentPrice From(
        DateOnly date,
        decimal open,
        decimal high,
        decimal low,
        decimal close,
        decimal adjustedClose,
        decimal volume) => new()
    {
        Date = date,
        Open = open,
        High = high,
        Low = low,
        Close = close,
        AdjustedClose = adjustedClose,
        Volume = volume
    };
}