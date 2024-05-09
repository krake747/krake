using System.Text.Json.Serialization;
using CsvHelper.Configuration;

namespace Krake.Cli.EODHistoricalData.EODHistoricalData.Models;

public sealed record Exchange
{
    [JsonIgnore] public string ExchangeId { get; init; } = string.Empty;
    [JsonPropertyName("Name")] public required string? Name { get; set; }
    [JsonPropertyName("Code")] public required string? Code { get; set; }
    [JsonPropertyName("OperatingMIC")] public required string? Mic { get; set; }
    [JsonPropertyName("Country")] public required string? Country { get; set; }
    [JsonPropertyName("Currency")] public required string? Currency { get; set; }
    [JsonPropertyName("CountryISO2")] public required string? CountryIso2 { get; set; }
    [JsonPropertyName("CountryISO3")] public required string? CountryIso3 { get; set; }
}

internal sealed class ExchangeMap : ClassMap<Exchange>
{
    public ExchangeMap()
    {
        Map(p => p.ExchangeId).Index(0).Name(nameof(Exchange.ExchangeId));
        Map(p => p.Name).Index(1).Name(nameof(Exchange.Name));
        Map(p => p.Code).Index(2).Name(nameof(Exchange.Code));
        Map(p => p.Mic).Index(3).Name(nameof(Exchange.Mic));
        Map(p => p.Country).Index(4).Name(nameof(Exchange.Country));
        Map(p => p.Currency).Index(5).Name(nameof(Exchange.Currency));
        Map(p => p.CountryIso2).Index(6).Name(nameof(Exchange.CountryIso2));
        Map(p => p.CountryIso3).Index(7).Name(nameof(Exchange.CountryIso3));
    }
}