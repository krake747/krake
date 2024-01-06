using System.Globalization;
using System.Text.RegularExpressions;
using Cocona;
using Krake.Application.Portfolios;
using Krake.Cli.Features.Comdirect;
using Krake.Core;
using Krake.Core.Functional;
using Krake.Infrastructure.IO.Models;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Krake.Cli.Features;

public sealed class ComdirectImporterApp(
    [FromService] IConfiguration config,
    [FromService] ILogger logger,
    [FromService] DirectoryManager directoryManager,
    [FromService] ComdirectFileManager comdirectFileManager)
{
    public void Run()
    {
        // Setup
        var inDirectory = directoryManager.In;
        var portfolioFile = inDirectory.EnumerateFiles()
            .First(x => CheckCsvExtension(x.Name) && x.Name.Contains(config["Files:Portfolio"]!));

        var positionDate = Path.GetFileNameWithoutExtension(portfolioFile.Name)
            .Split('_')[^2]
            .Pipe(x => DateOnly.ParseExact(x, "yyyyMMdd"));

        // Read data
        var fileReaderOptions = new FileReaderOptions
        {
            FileInfo = new FileInfo(portfolioFile.FullName),
            Delimiter = ';',
            HasHeaders = true,
            SkipLines = 5
        };

        var rawData = comdirectFileManager.Read(fileReaderOptions).Match(
            error => throw new Exception(error.Message),
            data => data);

        // Clean raw data
        var cleanData = rawData.Select(x =>
        {
            var keys = x.Keys.Select(UnescapeChars);
            var values = x.Values.Select(UnescapeChars);
            return keys.Zip(values, KeyValuePair.Create).ToDictionary();
        });

        // Normalize Data
        const string baseCurrency = "EUR";
        var portfolioOverrides = new PortfolioOverrides
        {
            PositionDate = positionDate,
            BaseCurrency = baseCurrency
        };

        var defaultFormatProvider = CultureInfo.CreateSpecificCulture(config["CultureInfo"] ?? string.Empty);
        var portfolioData = cleanData.Select(ComdirectPortfolioData.Create)
            .Select(x => x.MapToPortfolioData(portfolioOverrides, defaultFormatProvider))
            .ToArray();

        var portfolioDataLookup = portfolioData.ToLookup(k => (k.Isin, k.LocalCurrency), v => v);
        if (portfolioDataLookup.All(x => x.Key.LocalCurrency == baseCurrency) is false)
        {
            throw new Exception("Requires Fx Rates");
        }

        // Use Case #1: Aggregate data
        var aggregatedPortfolioData = portfolioDataLookup.Select(g =>
        {
            var posDate = g.First().PositionDate;
            var securityName = g.First().SecurityName;
            var totalShares = g.Sum(x => x.NumberOfShares);
            var totalValue = g.Sum(x => x.BaseReportedValue);
            var totalCostValue = g.Sum(x => x.BaseCostValue);
            return new Position(posDate, securityName, g.Key.Isin, g.Key.LocalCurrency, totalValue, totalShares,
                totalCostValue);
        }).ToArray();

        // Use Case #2: Calculate Total Portfolio Value
        var nav = aggregatedPortfolioData.Sum(x => x.TotalValue);
        logger.Information("Nav is {Nav:F2} {BaseCurrency}", nav, baseCurrency);
        foreach (var position in aggregatedPortfolioData)
        {
            logger.Information("Position: {Position}", position);
        }

        // Store Processed data in file
        var exportData = comdirectFileManager.Write(portfolioData).Match(
            error => throw new Exception(error.Message),
            data => data);

        var aggregatedExportData = comdirectFileManager.Write(aggregatedPortfolioData).Match(
            error => throw new Exception(error.Message),
            data => data);

        var outDirectory = directoryManager.Out;
        var exportFile = CreateFileInfo(outDirectory, $"{positionDate:O}_MasterPortfolio.csv");
        comdirectFileManager.Export(exportData, exportFile).Switch(
            error => throw new Exception(error.Message),
            _ => logger.Information("Successfully exported standard Portfolio data"));

        // Manage processed files
        _ = directoryManager.MoveFileToProcessed(portfolioFile, $"{positionDate:O}_processed_in.zip")
            .DoIfError(() => directoryManager.MoveFileToFailed(portfolioFile, $"{positionDate:O}_failed_in.zip"))
            .DoIfError(() => throw new Exception(Error.Failure().WithAttemptedValue(portfolioFile.Name).ToString()))
            .AsValue;

        // Data archiving In Directory
        _ = directoryManager.ZipInDirectoryToArchive($"{positionDate:O}_reports_in.zip")
            .DoIfError(() => throw new Exception(Error.Unexpected("Unable to create the zip file").ToString()))
            .AsValue;

        // Data archiving Out Directory
        var outZipReportsFileInfo = directoryManager.ZipOutDirectoryToArchive($"{positionDate:O}_reports_out.zip")
            .DoIfError(() => throw new Exception(Error.Unexpected("Unable to create the zip file").ToString()))
            .AsValue;

        // Post Cleanup - Delete Files
        inDirectory.EnumerateFiles()
            .Concat(outDirectory.EnumerateFiles())
            .ToList()
            .ForEach(x => x.Delete());
    }

    private static FileInfo CreateFileInfo(FileSystemInfo outDirectory, string fileName) =>
        new(Path.Combine(outDirectory.FullName, fileName));

    private static string UnescapeChars(string k) =>
        Regex.Unescape(k.AsSpan(1, k.Length - 2).ToString());

    private static bool CheckCsvExtension(string fileName) =>
        fileName.Contains(".csv", StringComparison.OrdinalIgnoreCase);
}