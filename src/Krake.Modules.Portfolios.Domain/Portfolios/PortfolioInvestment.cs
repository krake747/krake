using Krake.Core.Domain;

namespace Krake.Modules.Portfolios.Domain.Portfolios;

public sealed class PortfolioInvestment : Entity
{
    private PortfolioInvestment()
    {
    }

    public Guid PortfolioId { get; private set; }
    public Guid InstrumentId { get; private set; }
    public DateOnly PurchaseDate { get; private set; }
    public decimal PurchasePrice { get; private set; }
    public decimal Quantity { get; private set; }

    public static PortfolioInvestment From(
        Guid portfolioId,
        Guid instrumentId,
        DateOnly purchaseDate,
        decimal purchasePrice,
        decimal quantity) => new()
    {
        PortfolioId = portfolioId,
        InstrumentId = instrumentId,
        PurchaseDate = purchaseDate,
        PurchasePrice = purchasePrice,
        Quantity = quantity
    };
}