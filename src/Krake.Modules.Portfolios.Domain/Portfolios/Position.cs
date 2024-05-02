namespace Krake.Modules.Portfolios.Domain.Portfolios;

public record struct Position(
    DateOnly PositionDate,
    string Name,
    string Isin,
    string LocalCurrency,
    decimal TotalValue,
    decimal NumberOfShares,
    decimal TotalCostValue,
    decimal Performance);