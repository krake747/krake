using static Krake.Core.Monads.ErrorBase;

namespace Krake.Modules.Portfolios.Domain.Investments;

public static class InvestmentErrors
{
    private const string Prefix = nameof(Investment);

    public static Error AlreadyExists(Guid portfolioId, Guid securityId) =>
        Error.Problem(
                $"The investment already exists for portfolio {portfolioId} and security {securityId}",
                $"{Prefix}.{nameof(AlreadyExists)}")
            .WithAttemptedValue((portfolioId, securityId));
}