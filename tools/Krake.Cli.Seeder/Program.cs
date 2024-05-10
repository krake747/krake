using System.Globalization;
using System.Reflection;
using CsvHelper;
using Krake.Cli.EODHistoricalData.EODHistoricalData;
using Microsoft.Extensions.Configuration;

if (args.Length is 0)
{
    Console.WriteLine("Create seed file for:");
    Console.WriteLine("- exchanges");
    Console.WriteLine("- price-data");
    Console.Write("Enter: ");
    args = [Console.ReadLine()!];
}

var config = new ConfigurationBuilder()
    .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!)
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

var res = args switch
{
    ["exchanges"] => await SeedPortfoliosExchanges(config),
    ["price-data"] => await SeedInstrumentsPriceData(config),
    _ => throw new ArgumentException("Key not defined")
};

return;

static async ValueTask<bool> SeedPortfoliosExchanges(IConfiguration config)
{
    var http = new EodHistoricalDataHttpClient(config["ApiKey:EODHD"]!, new HttpClient());
    var portfoliosDir = Directory.CreateDirectory(config["SeedDirectory:Portfolios"]!);
    var exchanges = await http.GetExchangesAsync();

    await using var writer = new StreamWriter($"{portfoliosDir}/exchanges.csv");
    await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
    csv.WriteRecords(exchanges);

    Console.WriteLine("Created seed file exchanges.csv");
    return true;
}

static async ValueTask<bool> SeedInstrumentsPriceData(IConfiguration config)
{
    var http = new EodHistoricalDataHttpClient(config["ApiKey:EODHD"]!, new HttpClient());
    var portfoliosDir = Directory.CreateDirectory(config["SeedDirectory:Portfolios"]!);
    var instruments = new Dictionary<(string, string), Guid>
    {
        { ("ADS", "XETRA"), Guid.Parse("15B3C4A2-4053-4C11-AE8F-DE97909CB507") },
        { ("SIE", "XETRA"), Guid.Parse("9BA41F49-73D9-46B5-95C7-2EAB09A13806") },
        { ("BAS", "XETRA"), Guid.Parse("84815B16-EEB8-4784-A7CA-2211FC712675") },
        { ("MSFT", "US"), Guid.Parse("7B2ED9D3-5735-42F7-8814-F4CCB1B585BA") },
        { ("V", "US"), Guid.Parse("863DC0E1-2F64-4866-A1D8-9F62357E67DC") },
        { ("KO", "US"), Guid.Parse("BAB9E42C-BB30-4022-AEBA-C78344F2ADA6") },
    };

    var instrumentsPriceData = await http.DownloadHistoricalEndOfDayPriceDataAsync(
        instruments,
        new DateOnly(2023, 01, 01),
        new DateOnly(2024, 05, 08));

    var priceData = instrumentsPriceData.SelectMany(pd => pd.Value);

    await using var writer = new StreamWriter($"{portfoliosDir}/instruments_price_data.csv");
    await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
    csv.WriteRecords(priceData);

    Console.WriteLine("Created seed file instruments_price_data.csv");
    return true;
}