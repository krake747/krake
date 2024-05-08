using System.Globalization;
using System.Reflection;
using CsvHelper;
using Krake.Cli.EODHistoricalData.EODHistoricalData;
using Microsoft.Extensions.Configuration;

if (args.Length is 0)
{
    Console.WriteLine("Create seed file:");
    Console.WriteLine("- exchanges");
    args = [Console.ReadLine()!];
}

var config = new ConfigurationBuilder()
    .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!)
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

var res = args switch
{
    [var key and "exchanges"] => await SeedPortfoliosExchanges(config),
    _ => throw new ArgumentException("Key not defined")
};

var httpFactory = () => new HttpClient();
var eod = new EodHistoricalDataHttpClient(config["ApiKey:EODHD"]!, httpFactory());
var portfoliosDir = Directory.CreateDirectory(config["SeedDirectory:Portfolios"]!);
var exchanges = await eod.GetExchangesAsync();

await using var writer = new StreamWriter($"{portfoliosDir}/portfolios_exchanges.csv");
await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
csv.WriteRecords(exchanges);

return;

static async ValueTask<bool> SeedPortfoliosExchanges(IConfiguration config)
{
    var eod = new EodHistoricalDataHttpClient(config["ApiKey:EODHD"]!, new HttpClient());
    var portfoliosDir = Directory.CreateDirectory(config["SeedDirectory:Portfolios"]!);
    var exchanges = await eod.GetExchangesAsync();

    await using var writer = new StreamWriter($"{portfoliosDir}/portfolios_exchanges.csv");
    await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
    csv.WriteRecords(exchanges);

    return true;
}

// var historicalEndOfDayData = await eod
//     .GetHistoricalEndOfDayPriceDataAsync("MCD", "US", DateOnly.Parse("2023-01-01"), DateOnly.Parse("2024-05-07"));
//
// foreach (var value in historicalEndOfDayData)
// {
//     Console.WriteLine(value);
// }

// var alphaVantage = new AlphaVantageHttpClient(config["ApiKey:AlphaVantage"]!, httpFactory());
//
// var companyOverview = await alphaVantage.GetCompanyOverviewAsync("IBM");
// Console.WriteLine(companyOverview);