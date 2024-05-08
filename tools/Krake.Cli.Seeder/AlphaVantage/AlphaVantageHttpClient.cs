using System.Net.Http.Json;
using System.Text;

namespace Krake.Cli.EODHistoricalData.AlphaVantage;

internal sealed class AlphaVantageHttpClient
{
    private readonly string _apiToken;
    private readonly HttpClient _httpClient;

    public AlphaVantageHttpClient(string apiToken, HttpClient httpClient)
    {
        _apiToken = apiToken;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://www.alphavantage.co/");
    }

    public async ValueTask<Instrument?> GetCompanyOverviewAsync(string symbol)
    {
        var sb = new StringBuilder()
            .Append("query?")
            .Append("function=OVERVIEW&")
            .Append($"symbol={symbol}&")
            .Append($"apikey={_apiToken}");

        return await _httpClient.GetFromJsonAsync<Instrument>(sb.ToString());
    }

    public sealed record Instrument
    {
        public required string Symbol { get; init; }
        public required string AssetType { get; init; }
        public required string Name { get; init; }
        public required string Exchange { get; init; }
        public required string Currency { get; init; }
        public required string Country { get; init; }
        public required string Sector { get; init; }
    }
}