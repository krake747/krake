using Krake.Modules.Portfolios.Domain.Instruments;

namespace Krake.Modules.Portfolios.Application.Instruments;

public interface IInstrumentRepository : IReadOnlyInstrumentRepository
{
    Task<bool> CreateAsync(Instrument instrument, CancellationToken token = default);
}

public interface IReadOnlyInstrumentRepository
{
    Task<IEnumerable<Instrument>> ListAsync(CancellationToken token = default);
    Task<Instrument?> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<IEnumerable<InstrumentPrice>> ListPricesByIdAsync(Guid id, CancellationToken token = default);
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default);
}