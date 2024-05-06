using Krake.Core.Application.Messaging;
using Krake.Core.Monads;
using Krake.Modules.Portfolios.Application.Instruments;
using Krake.Modules.Portfolios.Domain.Portfolios;
using static Krake.Core.Monads.ErrorBase;

namespace Krake.Modules.Portfolios.Application.Portfolios.AddPortfolioInvestment;

public sealed record AddPortfolioInvestmentCommand(
    Guid PortfolioId,
    Guid InstrumentId,
    DateOnly PurchaseDate,
    decimal PurchasePrice,
    decimal Quantity)
    : ICommand<ErrorBase, Success>;

internal sealed class AddPortfolioInvestmentCommandHandler(
    TimeProvider timeProvider,
    IPortfolioRepository portfolioRepository,
    IReadOnlyInstrumentRepository instrumentRepository)
    : ICommandHandler<AddPortfolioInvestmentCommand, ErrorBase, Success>
{
    public async Task<Result<ErrorBase, Success>> Handle(AddPortfolioInvestmentCommand request,
        CancellationToken token = default)
    {
        var portfolioExistsTask = portfolioRepository.ExistsByIdAsync(request.PortfolioId, token);
        var instrumentExistsTask = instrumentRepository.ExistsByIdAsync(request.InstrumentId, token);

        var results = await Task.WhenAll([portfolioExistsTask, instrumentExistsTask]);

        if (results[0] is false)
        {
            return PortfolioErrors.NotFound(request.PortfolioId);
        }

        if (results[1] is false)
        {
            return PortfolioErrors.InstrumentNotFound(request.InstrumentId);
        }

        if (request.PurchaseDate > DateOnly.FromDateTime(timeProvider.GetUtcNow().DateTime))
        {
            return PortfolioInvestmentErrors.PurchaseDateInFuture;
        }

        var portfolioInvestment = PortfolioInvestment.From(
            request.PortfolioId,
            request.InstrumentId,
            request.PurchaseDate,
            request.PurchasePrice,
            request.Quantity);

        var added = await portfolioRepository.AddPortfolioInvestmentAsync(portfolioInvestment, token);

        return added is false ? Error.Conflict() : Ok.Success;
    }
}