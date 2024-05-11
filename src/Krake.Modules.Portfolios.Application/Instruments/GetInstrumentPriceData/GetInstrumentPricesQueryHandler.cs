using Krake.Core.Application.Messaging;
using Krake.Core.Monads;
using Krake.Modules.Portfolios.Domain.Instruments;

namespace Krake.Modules.Portfolios.Application.Instruments.GetInstrumentPriceData;

public sealed record GetInstrumentPricesQuery(Guid InstrumentId)
    : IQuery<ErrorBase, InstrumentPricesResponse>;

internal sealed class GetInstrumentPricesQueryHandler(IReadOnlyInstrumentRepository instrumentRepository)
    : IQueryHandler<GetInstrumentPricesQuery, ErrorBase, InstrumentPricesResponse>
{
    public async Task<Result<ErrorBase, InstrumentPricesResponse>> Handle(GetInstrumentPricesQuery request,
        CancellationToken token = default)
    {
        var exists = await instrumentRepository.ExistsByIdAsync(request.InstrumentId, token);
        if (exists is false)
        {
            return InstrumentErrors.NotFound(request.InstrumentId);
        }

        var prices = await instrumentRepository.ListPricesByIdAsync(request.InstrumentId, token);

        return new InstrumentPricesResponse(request.InstrumentId, prices);
    }
}