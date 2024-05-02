using Krake.Core.Application.Messaging;
using Krake.Core.Monads;
using Krake.Modules.Portfolios.Domain.Portfolios;

namespace Krake.Modules.Portfolios.Application.Portfolios.UpdatePortfolio;

internal sealed class UpdatePortfolioCommandHandler(IPortfolioRepository portfolioRepository)
    : ICommandHandler<UpdatePortfolioCommand, ErrorBase, Success>
{
    public async Task<Result<ErrorBase, Success>> Handle(UpdatePortfolioCommand request,
        CancellationToken token = default)
    {
        var portfolio = await portfolioRepository.GetByIdAsync(request.PortfolioId, token);
        if (portfolio is null)
        {
            return PortfolioErrors.NotFound(request.PortfolioId);
        }

        var success = portfolio.ChangeName(request.Name);

        _ = await portfolioRepository.UpdateAsync(portfolio, token);

        return success;
    }
}