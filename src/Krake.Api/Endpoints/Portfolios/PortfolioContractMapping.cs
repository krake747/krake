using Krake.Application.Portfolios;
using Krake.Contracts.Portfolios.Requests;
using Krake.Contracts.Portfolios.Responses;

namespace Krake.Api.Endpoints.Portfolios;

public static class PortfolioContractMapping
{
    public static CreatePortfolio MapToCreate(this CreatePortfolioRequest request, Guid id) =>
        new(id, request.Name);
    
    public static UpdatePortfolio MapToUpdate(this UpdatePortfolioRequest request, Portfolio portfolio) =>
        new(request.Name ?? portfolio.Name);

    public static PortfolioResponse MapToResponse(this Portfolio portfolio) => new()
    {
        Id = portfolio.Id,
        Name = portfolio.Name
    };

    public static PortfolioResponse MapToResponse(this CreatePortfolio portfolio) => new()
    {
        Id = portfolio.Id,
        Name = portfolio.Name
    };

    public static PortfoliosResponse MapToResponse(this IEnumerable<Portfolio> portfolios) => new()
    {
        Items = portfolios.Select(MapToResponse)
    };
}