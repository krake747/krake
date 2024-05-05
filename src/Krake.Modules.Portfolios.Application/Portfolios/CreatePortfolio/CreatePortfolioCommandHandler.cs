using Krake.Core.Application.Messaging;
using Krake.Core.Monads;
using Krake.Modules.Portfolios.Domain.Portfolios;

namespace Krake.Modules.Portfolios.Application.Portfolios.CreatePortfolio;

public sealed record CreatePortfolioCommand(string Name, string Currency)
    : ICommand<ErrorBase, Guid>;

internal sealed class CreatePortfolioCommandHandler(IPortfolioRepository portfolioRepository)
    : ICommandHandler<CreatePortfolioCommand, ErrorBase, Guid>
{
    public async Task<Result<ErrorBase, Guid>> Handle(CreatePortfolioCommand request,
        CancellationToken token = default)
    {
        var portfolio = Portfolio.From(request.Name, request.Currency);

        _ = await portfolioRepository.CreateAsync(portfolio, token);

        return portfolio.Id;
    }
}