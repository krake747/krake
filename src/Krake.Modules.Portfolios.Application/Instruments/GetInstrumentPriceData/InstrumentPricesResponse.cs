using Krake.Modules.Portfolios.Domain.Instruments;

namespace Krake.Modules.Portfolios.Application.Instruments.GetInstrumentPriceData;

public sealed record InstrumentPricesResponse(Guid InstrumentId, IEnumerable<InstrumentPrice> Prices);