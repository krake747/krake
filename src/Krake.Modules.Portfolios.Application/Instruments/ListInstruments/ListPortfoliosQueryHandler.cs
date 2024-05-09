using Dapper;
using Krake.Core.Application.Messaging;
using Krake.Modules.Portfolios.Application.Instruments.GetInstrument;

namespace Krake.Modules.Portfolios.Application.Instruments.ListInstruments;

public sealed record ListInstrumentsQuery
    : IQuery<IReadOnlyCollection<InstrumentResponse>>;

internal sealed class ListPortfoliosQueryHandler(IReadOnlyInstrumentRepository instrumentRepository)
    : IQueryHandler<ListInstrumentsQuery, IReadOnlyCollection<InstrumentResponse>>
{
    public async Task<IReadOnlyCollection<InstrumentResponse>> Handle(ListInstrumentsQuery request,
        CancellationToken token = default)
    {
        var instruments = await instrumentRepository.ListAsync(token);
        return instruments.Select(p => new InstrumentResponse(
                p.Id,
                p.Name,
                p.Currency,
                p.Country,
                p.Mic,
                p.Sector,
                p.Symbol,
                p.Isin))
            .AsList();
    }
}