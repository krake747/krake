using Krake.Application.Portfolios;
using Krake.Contracts.Portfolios.Responses;

namespace Krake.Api.Endpoints.Portfolios;

public static class PortfolioContractMapping
{
    public static PortfolioResponse MapToResponse(this Portfolio portfolio) => new()
    {
        Id = portfolio.Id,
        Name = portfolio.Name
    };
    
    public static PortfoliosResponse MapToResponse(this IEnumerable<Portfolio> portfolios) => new()
    {
        Items = portfolios.Select(MapToResponse)
    };
}