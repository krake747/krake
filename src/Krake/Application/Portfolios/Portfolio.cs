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

public sealed class PortfolioData
{
    public required DateOnly PositionDate { get; init; }
    public required string PortfolioBaseCurrency { get; init; }
    public required string SecurityName { get; init; }
    public required string Isin { get; init; }
    public required string LocalCurrency { get; init; }
    public required decimal NumberOfShares { get; init; }
    public required decimal LocalPrice { get; init; }
    public required decimal BaseCostPrice { get; init; }
    public required decimal BaseCostValue { get; init; }
    public required decimal BaseReportedValue { get; init; }
    public required DateOnly BuyDate { get; init; }
}

public sealed class PortfolioOverrides
{
    public DateOnly PositionDate { get; set; }
    public string? BaseCurrency { get; set; }
}

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