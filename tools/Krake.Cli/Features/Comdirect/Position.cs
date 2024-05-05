namespace Krake.Cli.Features.Comdirect;

public record struct Position(
    DateOnly PositionDate,
    string Name,
    string Isin,
    string LocalCurrency,
    decimal TotalValue,
    decimal NumberOfShares,
    decimal TotalCostValue,
    decimal Performance);