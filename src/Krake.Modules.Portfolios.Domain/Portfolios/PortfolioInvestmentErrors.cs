using static Krake.Core.Monads.ErrorBase;

namespace Krake.Modules.Portfolios.Domain.Portfolios;

public static class PortfolioInvestmentErrors
{
    private const string Prefix = nameof(Portfolio);

    public static readonly Error PurchaseDateInFuture =
        Error.Problem(
            "The purchase date is in the future",
            $"{Prefix}.{nameof(PurchaseDateInFuture)}");
}