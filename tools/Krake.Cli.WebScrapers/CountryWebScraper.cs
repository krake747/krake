using System.Text.Json;
using System.Text.Json.Nodes;

namespace Krake.Cli.WebScrapers;

using CountriesLookup = Dictionary<string, string>;

internal static class CountryWebScraper
{
    public static async Task<CountriesLookup> ScrapeFromGitHub(Func<HttpClient> httpFactory)
    {
        const string baseUri = "https://raw.githubusercontent.com/lukes/ISO-3166-Countries-with-Regional-Codes/master";
        var uri = new Uri($"{baseUri}/all/all.json");
        using var http = httpFactory();
        var response = await http.GetAsync(uri);
        var content = await response.Content.ReadAsStringAsync();

        var jsonObjects = JsonSerializer.Deserialize<IEnumerable<JsonObject>>(content) ?? [];
        var countries = new CountriesLookup();
        foreach (var obj in jsonObjects)
        {
            var code = obj["alpha-2"]?.GetValue<string>() ?? string.Empty;
            var name = obj["name"]?.GetValue<string>() ?? string.Empty;
            _ = countries.TryAdd(code, name);
        }

        return countries;
    }
}