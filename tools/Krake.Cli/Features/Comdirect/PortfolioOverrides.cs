namespace Krake.Cli.Features.Comdirect;

public sealed class PortfolioOverrides
{
    public DateOnly PositionDate { get; set; }
    public string? BaseCurrency { get; set; }
}