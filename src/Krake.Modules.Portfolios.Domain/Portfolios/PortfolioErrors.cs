using static Krake.Core.Monads.ErrorBase;

namespace Krake.Modules.Portfolios.Domain.Portfolios;

public static class PortfolioErrors
{
    private const string Prefix = nameof(Portfolio);

    public static Error NotFound(Guid portfolioId) =>
        Error.NotFound(
                $"The portfolio with the identifier {portfolioId} was not found",
                $"{Prefix}.{nameof(NotFound)}")
            .WithAttemptedValue(portfolioId);

    public static Error InstrumentNotFound(Guid instrumentId) =>
        Error.NotFound(
                $"The instrument with the identifier {instrumentId} was not found",
                $"{Prefix}.{nameof(InstrumentNotFound)}")
            .WithAttemptedValue(instrumentId);
}