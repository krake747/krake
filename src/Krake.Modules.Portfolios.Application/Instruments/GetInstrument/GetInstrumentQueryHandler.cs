using Krake.Core.Application.Messaging;
using Krake.Core.Monads;
using Krake.Modules.Portfolios.Domain.Instruments;

namespace Krake.Modules.Portfolios.Application.Instruments.GetInstrument;

public sealed record GetInstrumentQuery(Guid InstrumentId)
    : IQuery<ErrorBase, InstrumentResponse>;

internal sealed class GetInstrumentQueryHandler(IReadOnlyInstrumentRepository instrumentRepository)
    : IQueryHandler<GetInstrumentQuery, ErrorBase, InstrumentResponse>
{
    public async Task<Result<ErrorBase, InstrumentResponse>> Handle(GetInstrumentQuery request,
        CancellationToken token = default)
    {
        var exists = await instrumentRepository.ExistsByIdAsync(request.InstrumentId, token);
        if (exists is false)
        {
            return InstrumentErrors.NotFound(request.InstrumentId);
        }

        var portfolio = await instrumentRepository.GetByIdAsync(request.InstrumentId, token);

        return new InstrumentResponse(portfolio!.Id, portfolio.Name, portfolio.Currency);
    }
}