namespace Krake.Contracts.Portfolios.Responses;

public sealed class PortfoliosResponse
{
    public IEnumerable<PortfolioResponse> Items { get; init; } = Enumerable.Empty<PortfolioResponse>();
}