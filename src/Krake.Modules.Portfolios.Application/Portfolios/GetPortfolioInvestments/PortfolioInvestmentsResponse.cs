namespace Krake.Modules.Portfolios.Application.Portfolios.GetPortfolioInvestments;

public sealed class PortfolioInvestmentsResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Currency { get; init; }
    public required decimal TotalValue { get; init; }
    public IList<PortfolioInvestmentResponse> Investments { get; init; } = [];
}

public sealed class PortfolioInvestmentResponse
{
    public required Guid InstrumentId { get; init; }
    public required string InstrumentName { get; init; }
    public required string InstrumentCurrency { get; init; }
    public required string InstrumentCountry { get; init; }
    public required string InstrumentMic { get; init; }
    public required string InstrumentSector { get; init; }
    public required string InstrumentSymbol { get; init; }
    public required string InstrumentIsin { get; init; }
    public required DateOnly PurchaseDate { get; init; }
    public required decimal PurchasePrice { get; init; }
    public required decimal Quantity { get; init; }
}