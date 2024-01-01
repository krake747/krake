namespace Krake.Contracts.Portfolios.Responses;

public sealed class PortfolioResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}