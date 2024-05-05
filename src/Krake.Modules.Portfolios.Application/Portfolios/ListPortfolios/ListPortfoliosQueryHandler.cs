using Dapper;
using Krake.Core.Application.Messaging;
using Krake.Modules.Portfolios.Application.Portfolios.GetPortfolio;

namespace Krake.Modules.Portfolios.Application.Portfolios.ListPortfolios;

public sealed record ListPortfoliosQuery
    : IQuery<IReadOnlyCollection<PortfolioResponse>>;

internal sealed class ListPortfoliosQueryHandler(IReadOnlyPortfolioRepository portfolioRepository)
    : IQueryHandler<ListPortfoliosQuery, IReadOnlyCollection<PortfolioResponse>>
{
    public async Task<IReadOnlyCollection<PortfolioResponse>> Handle(ListPortfoliosQuery request,
        CancellationToken token = default)
    {
        var portfolios = await portfolioRepository.ListAsync(token);
        return portfolios.Select(p => new PortfolioResponse(p.Id, p.Name, p.Currency)).AsList();
    }
}