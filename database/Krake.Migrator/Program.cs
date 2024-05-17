using System.Globalization;
using System.Numerics;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;
using Dapper;
using DbUp;
using DbUp.Engine;
using DbUp.Helpers;
using Krake.Database;
using Krake.Database.Models;
using Krake.Migrator.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
var resourcesPath = Path.Combine(basePath, "Resources");

var config = new ConfigurationBuilder()
    .SetBasePath(basePath)
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

var connectionString = config.GetConnectionString("SqlDatabase")!;
var connectionFactory = new SqlConnectionFactory(connectionString);

// Wait for Database connection

WaitForDatabase(connectionFactory);

// Create initial tables

var migrator =
    DeployChanges.To
        .SqlDatabase(connectionString)
        .WithScriptsEmbeddedInAssemblies(
            [Assembly.GetExecutingAssembly(), typeof(IKrakeDatabaseScriptsMarker).Assembly], s => s.Contains("Create"))
        .JournalTo(new NullJournal())
        .LogToConsole()
        .Build();

var migration = migrator.PerformUpgrade();
if (migration.Successful is false)
{
    LogDbUpError(migration);
    Environment.Exit(-1);
}

LogSuccess();

// Seed definition tables

var definitions = Directory.GetFiles(resourcesPath, "*definition*");
foreach (var resource in definitions)
{
    using var reader = new StreamReader(resource);
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

    var fileName = Path.GetFileName(resource);
    var rows = fileName.Split('_', 2)[^1] switch
    {
        "exchanges.csv" => BulkCopy<ExchangeMap, Exchange>(connectionFactory, csv, "KrakeDB.Portfolios.Exchanges"),
        _ => throw new Exception()
    };

    if (rows is 0)
    {
        LogNotFound(fileName);
        Environment.Exit(-1);
    }

    LogRows(fileName, rows);
}

// Seed main tables

var sqlSeeder =
    DeployChanges.To
        .SqlDatabase(connectionString)
        .WithScriptsEmbeddedInAssemblies(
            [Assembly.GetExecutingAssembly(), typeof(IKrakeDatabaseScriptsMarker).Assembly], s => s.Contains("Seed"))
        .JournalTo(new NullJournal())
        .LogToConsole()
        .Build();

var seeds = sqlSeeder.PerformUpgrade();
if (seeds.Successful is false)
{
    LogDbUpError(seeds);
    Environment.Exit(-1);
}

LogSuccess();

// Seed secondary tables

var secondaries = Directory.GetFiles(resourcesPath, "*secondary*");
foreach (var resource in secondaries)
{
    using var reader = new StreamReader(resource);
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

    var fileName = Path.GetFileName(resource);
    var rows = fileName.Split('_', 2)[^1] switch
    {
        "instrument_prices.csv" => BulkCopy<PriceDataMap, PriceData>(connectionFactory, csv,
            "KrakeDB.Portfolios.InstrumentPrices", SqlBulkCopyOptions.KeepIdentity),
        _ => throw new Exception()
    };

    if (rows is 0)
    {
        LogNotFound(fileName);
        Environment.Exit(-1);
    }

    LogRows(fileName, rows);
}

return 0;

static void LogSuccess()
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Success!");
    Console.ResetColor();
}

static void LogRows<T>(string fileName, T rows) where T : INumber<T>
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Seeded {fileName} with {rows} Rows");
    Console.ResetColor();
}

static void LogNotFound(string fileName)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"No rows found for {fileName}");
    Console.ResetColor();
}

static void LogDbUpError(DatabaseUpgradeResult databaseUpgradeResult)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(databaseUpgradeResult);
    Console.ResetColor();
}

static long BulkCopy<TMap, T>(SqlConnectionFactory connectionFactory, CsvReader csv, string tableName,
    SqlBulkCopyOptions options = SqlBulkCopyOptions.Default)
    where TMap : ClassMap
{
    csv.Context.RegisterClassMap<TMap>();
    var records = csv.GetRecords<T>().ToArray();
    return SqlConnectionExtensions.BulkInsert(connectionFactory, records, tableName, options);
}

static void WaitForDatabase(SqlConnectionFactory connectionFactory)
{
    for (var i = 1; i <= 50; i++)
    {
        try
        {
            using (var connection = connectionFactory.CreateConnection())
            {
                connection.ExecuteScalar("SELECT 1");
            }

            Console.WriteLine("SQL Server is ready.");
            break;
        }
        catch (SqlException)
        {
            Console.WriteLine("Not ready yet...");
            Thread.Sleep(1000); // Sleep for 1 second
        }
    }
}