using System.Net.Http.Json;
using System.Text;
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

    public async ValueTask<Dictionary<Guid, IEnumerable<PriceData>>> DownloadHistoricalEndOfDayPriceDataAsync(
        Dictionary<(string, string), Guid> instruments,
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken token = default)
    {
        Dictionary<Guid, IEnumerable<PriceData>> instrumentsPriceData = [];
        foreach (var ((symbol, exchange), instrumentId) in instruments)
        {
            var priceData = await GetHistoricalEndOfDayPriceDataAsync(symbol, exchange, fromDate, toDate, token: token);
            instrumentsPriceData[instrumentId] = priceData.Select(p => p with { InstrumentId = instrumentId });
        }

        return instrumentsPriceData;
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

        var priceData = await _httpClient.GetFromJsonAsync<IEnumerable<PriceData>>(sb.ToString(), token) ?? [];

        return priceData.Select(TrimStrings);
    }

    public async ValueTask<IEnumerable<Exchange>> GetExchangesAsync(CancellationToken token = default)
    {
        var sb = new StringBuilder()
            .Append("exchanges-list/")
            .Append("?fmt=json&")
            .Append($"api_token={_apiToken}");

        var exchanges = (await _httpClient.GetFromJsonAsync<IEnumerable<Exchange>>(sb.ToString(), token) ?? [])
            .ToList();

        // using var reader = new StreamReader("C:\\Users\\kraem\\krake\\database\\portfolios\\portfolios_exchanges.csv");
        // using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        // csv.Context.RegisterClassMap<ExchangeMap>();
        // var exchanges = csv.GetRecords<Exchange>();

        var finalExchanges = exchanges.Select(TrimStrings)
            .Where(e => e.Country is not "Unknown" || e.Currency is not "Unknown")
            .Where(e => string.IsNullOrEmpty(e.Mic) is false)
            .Where(e => string.IsNullOrEmpty(e.CountryIso2) is false)
            .Where(e => string.IsNullOrEmpty(e.CountryIso3) is false)
            .DistinctBy(e => e.Mic)
            .ToList();

        List<Exchange> addExchanges = [];
        List<Exchange> deleteExchanges = [];
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
                addExchanges.Add(exchange with { Mic = mics.Pop() });
            }

            deleteExchanges.Add(exchange);
        }

        foreach (var exchange in deleteExchanges)
        {
            finalExchanges.Remove(exchange);
        }

        return [..finalExchanges, ..addExchanges];
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