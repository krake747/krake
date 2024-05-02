namespace Krake.Modules.Portfolios.Domain.Portfolios;

public sealed class PortfolioData
{
    public required DateOnly PositionDate { get; init; }
    public required string PortfolioBaseCurrency { get; init; }
    public required string SecurityName { get; init; }
    public required string Isin { get; init; }
    public required string LocalCurrency { get; init; }
    public required decimal NumberOfShares { get; init; }
    public required decimal LocalPrice { get; init; }
    public required decimal BaseCostPrice { get; init; }
    public required decimal BaseCostValue { get; init; }
    public required decimal BaseReportedValue { get; init; }
    public required DateOnly BuyDate { get; init; }
}