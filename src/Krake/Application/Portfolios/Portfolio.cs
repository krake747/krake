namespace Krake.Application.Portfolios;

public sealed record Portfolio
{
    private Portfolio()
    {
    }
    
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    
    public static Portfolio From(PortfolioDto portfolioDto) => new()
    {
        Id = portfolioDto.Id ?? Guid.NewGuid(),
        Name = portfolioDto.Name
    };
}

public sealed class PortfolioDto
{
    public Guid? Id { get; set; }
    public required string Name { get; init; }
}

public sealed record CreatePortfolio(string Name);

public sealed record UpdatePortfolio(string? Name);

public static class PortfolioMapping
{
    public static PortfolioDto MapToDto(this CreatePortfolio create) => new()
    {
        Name = create.Name
    };
    
    public static PortfolioDto MapToDto(this UpdatePortfolio update, Portfolio portfolio) => new()
    {
        Id = portfolio.Id,
        Name = update.Name ?? portfolio.Name
    };
}
