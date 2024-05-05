using static Krake.Core.Monads.ErrorBase;

namespace Krake.Modules.Portfolios.Domain.Instruments;

public static class InstrumentErrors
{
    private const string Prefix = nameof(Instrument);

    public static Error NotFound(Guid securityId) =>
        Error.NotFound(
                $"The security with the identifier {securityId} was not found",
                $"{Prefix}.{nameof(NotFound)}")
            .WithAttemptedValue(securityId);
}