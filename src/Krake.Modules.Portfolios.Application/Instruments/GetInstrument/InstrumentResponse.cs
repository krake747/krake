namespace Krake.Modules.Portfolios.Application.Instruments.GetInstrument;

public sealed record InstrumentResponse(
    Guid Id,
    string Name,
    string Currency,
    string Country,
    string Mic,
    string Sector,
    string Symbol,
    string Isin);