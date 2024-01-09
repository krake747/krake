using Krake.Application.Portfolios;
using Krake.Core.Extensions;
using Krake.Infrastructure.IO.Common.Attributes;

namespace Krake.Cli.Features.Comdirect;

public sealed class ComdirectPortfolioData
{
    [ColumnName("Stück")] public required string NumberOfShares { get; init; }
    [ColumnName("Bezeichnung")] public required string Name { get; init; }
    [ColumnName("ISIN")] public required string Isin { get; init; }
    [ColumnName("Whg.")] public required string LocalCurrency { get; init; }
    [ColumnName("Aktuell")] public required string LocalPrice { get; init; }
    [ColumnName("Kaufkurs in EUR")] public required string CostPriceEur { get; init; }
    [ColumnName("Kaufwert in EUR")] public required string CostValueEur { get; init; }
    [ColumnName("Wert in EUR")] public required string ReportedValueEur { get; init; }
    [ColumnName("Kaufdatum")] public required string BuyDate { get; init; }

    public static ComdirectPortfolioData Create(Dictionary<string, string> dictionary) => new()
    {
        NumberOfShares = dictionary["Stück"],
        Name = dictionary["Bezeichnung"],
        Isin = dictionary["ISIN"],
        LocalCurrency = dictionary["Whg."],
        LocalPrice = dictionary["Aktuell"],
        CostPriceEur = dictionary["Kaufkurs in EUR"],
        CostValueEur = dictionary["Kaufwert in EUR"],
        ReportedValueEur = dictionary["Wert in EUR"],
        BuyDate = dictionary["Kaufdatum"]
    };
}

public static class ComdirectPortfolioDataExtensions
{
    public static PortfolioData MapToPortfolioData(this ComdirectPortfolioData data, PortfolioOverrides overrides,
        IFormatProvider? formatProvider = default) => new()
    {
        PositionDate = overrides.PositionDate,
        PortfolioBaseCurrency = overrides.BaseCurrency ?? string.Empty,
        SecurityName = data.Name,
        Isin = data.Isin,
        LocalCurrency = data.LocalCurrency,
        NumberOfShares = data.NumberOfShares.ToValueOrDefault(0m, formatProvider),
        LocalPrice = data.LocalPrice.ToValueOrDefault(0m, formatProvider),
        BaseCostPrice = data.CostPriceEur.ToValueOrDefault(0m, formatProvider),
        BaseCostValue = data.CostValueEur.ToValueOrDefault(0m, formatProvider),
        BaseReportedValue = data.ReportedValueEur.ToValueOrDefault(0m, formatProvider),
        BuyDate = DateOnly.ParseExact(data.BuyDate, "dd.mm.yy")
    };
}