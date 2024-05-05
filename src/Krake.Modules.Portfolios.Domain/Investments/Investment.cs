using Krake.Core.Domain;

namespace Krake.Modules.Portfolios.Domain.Investments;

public sealed class Investment : Entity
{
    private Investment()
    {
    }

    public Guid Id { get; private set; }
    public Guid PortfolioId { get; private set; }
    public Guid SecurityId { get; private set; }
    public DateOnly PurchaseDate { get; private set; }
    public DateOnly PurchasePrice { get; private set; }
    public decimal Quantity { get; private set; }

    public static Investment From(
        Guid portfolioId,
        Guid securityId,
        DateOnly purchaseDate,
        DateOnly purchasePrice,
        decimal quantity) => new()
    {
        Id = Guid.NewGuid(),
        PortfolioId = portfolioId,
        SecurityId = securityId,
        PurchaseDate = purchaseDate,
        PurchasePrice = purchasePrice,
        Quantity = quantity
    };
}