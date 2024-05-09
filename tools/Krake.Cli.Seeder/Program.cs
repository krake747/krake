using System.Globalization;
using System.Reflection;
using CsvHelper;
using Krake.Cli.EODHistoricalData.EODHistoricalData;
using Microsoft.Extensions.Configuration;

if (args.Length is 0)
{
    Console.WriteLine("Create seed file for:");
    Console.WriteLine("- exchanges");
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
    _ => throw new ArgumentException("Key not defined")
};

return;

static async ValueTask<bool> SeedPortfoliosExchanges(IConfiguration config)
{
    var http = new EodHistoricalDataHttpClient(config["ApiKey:EODHD"]!, new HttpClient());
    var portfoliosDir = Directory.CreateDirectory(config["SeedDirectory:Portfolios"]!);
    var exchanges = await http.GetExchangesAsync();

    await using var writer = new StreamWriter($"{portfoliosDir}/portfolios_exchanges.csv");
    await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
    csv.WriteRecords(exchanges);

    Console.WriteLine("Created seed file portfolios_exchanges.csv");
    return true;
}