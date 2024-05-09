using Krake.Core.Application.Messaging;
using Krake.Core.Monads;
using Krake.Modules.Portfolios.Domain.Instruments;

namespace Krake.Modules.Portfolios.Application.Instruments.CreateInstrument;

public sealed record CreateInstrumentCommand(
    string Name,
    string Currency,
    string Country,
    string Mic,
    string Sector,
    string Symbol,
    string Isin)
    : ICommand<ErrorBase, Guid>;

internal sealed class CreateInstrumentCommandHandler(IInstrumentRepository instrumentRepository)
    : ICommandHandler<CreateInstrumentCommand, ErrorBase, Guid>
{
    public async Task<Result<ErrorBase, Guid>> Handle(CreateInstrumentCommand request,
        CancellationToken token = default)
    {
        var instrument = Instrument.From(
            request.Name,
            request.Currency,
            request.Country,
            request.Mic,
            request.Sector,
            request.Symbol,
            request.Isin);

        _ = await instrumentRepository.CreateAsync(instrument, token);

        return instrument.Id;
    }
}