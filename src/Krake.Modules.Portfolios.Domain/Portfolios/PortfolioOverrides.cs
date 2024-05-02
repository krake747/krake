namespace Krake.Modules.Portfolios.Domain.Portfolios;

public sealed class PortfolioOverrides
{
    public DateOnly PositionDate { get; set; }
    public string? BaseCurrency { get; set; }
}