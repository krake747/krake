using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using CsvHelper;
using Krake.Cli.EODHistoricalData.EODHistoricalData.Models;

namespace Krake.Cli.EODHistoricalData.EODHistoricalData;

internal sealed class EodHistoricalDataHttpClient
{
    private readonly string _apiToken;
    private readonly HttpClient _httpClient;

    public EodHistoricalDataHttpClient(string apiToken, HttpClient httpClient)
    {
        _apiToken = apiToken;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://eodhd.com/api/");
    }

    public async ValueTask<IEnumerable<PriceData>> GetHistoricalEndOfDayPriceDataAsync(
        string symbol,
        string exchange,
        DateOnly fromDate,
        DateOnly toDate,
        string? period = "d",
        string? format = "json",
        CancellationToken token = default)
    {
        var sb = new StringBuilder()
            .Append("eod/")
            .Append($"{symbol}.{exchange}")
            .Append('?')
            .Append($"from={fromDate:O}&")
            .Append($"toDate={toDate:O}&")
            .Append($"period={period}&")
            .Append($"fmt={format}&")
            .Append($"api_token={_apiToken}");

        return await _httpClient.GetFromJsonAsync<IEnumerable<PriceData>>(sb.ToString(), token) ?? [];
    }

    public async ValueTask<IEnumerable<Exchange>> GetExchangesAsync(CancellationToken token = default)
    {
        var sb = new StringBuilder()
            .Append("exchanges-list/")
            .Append("?fmt=json&")
            .Append($"api_token={_apiToken}");

        var exchanges = (await _httpClient.GetFromJsonAsync<IEnumerable<Exchange>>(sb.ToString(), token) ?? [])
            .ToList();

        var finalExchanges = exchanges.Select(TrimStrings)
            .Where(e => e.Country is not "Unknown" || e.Currency is not "Unknown")
            .Where(e => string.IsNullOrEmpty(e.Mic) is false)
            .Where(e => string.IsNullOrEmpty(e.CountryIso2) is false)
            .Where(e => string.IsNullOrEmpty(e.CountryIso3) is false)
            .ToList();

        List<Exchange> additionalExchanges = [];
        List<Exchange> clearExchanges = [];
        foreach (var exchange in finalExchanges)
        {
            var mic = exchange.Mic!.Split(',', StringSplitOptions.TrimEntries);
            if (mic.Length < 2)
            {
                continue;
            }

            var mics = new Stack<string>(mic);
            while (mics.Count is not 0)
            {
                var newExchange = exchange with { Mic = mics.Pop() };
                additionalExchanges.Add(newExchange);
            }

            clearExchanges.Add(exchange);
        }

        foreach (var exchange in clearExchanges)
        {
            finalExchanges.Remove(exchange);
        }

        return [..finalExchanges, ..additionalExchanges];
    }

    private static T TrimStrings<T>(T obj)
        where T : notnull
    {
        var stringProperties = obj.GetType()
            .GetProperties()
            .Where(p => p.PropertyType == typeof(string));

        foreach (var stringProperty in stringProperties)
        {
            var currentValue = stringProperty.GetValue(obj, null)?.ToString();
            if (currentValue is null)
            {
                continue;
            }

            stringProperty.SetValue(obj, currentValue.Trim(), null);
        }

        return obj;
    }
}