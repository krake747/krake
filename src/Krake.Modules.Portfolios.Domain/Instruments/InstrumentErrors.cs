using static Krake.Core.Monads.ErrorBase;

namespace Krake.Modules.Portfolios.Domain.Instruments;

public static class InstrumentErrors
{
    private const string Prefix = nameof(Instrument);

    public static Error NotFound(Guid instrumentId) =>
        Error.NotFound(
                $"The instrument with the identifier {instrumentId} was not found",
                $"{Prefix}.{nameof(NotFound)}")
            .WithAttemptedValue(instrumentId);
}