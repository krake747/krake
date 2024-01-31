using System.Text;
using Krake.Cli.Features.Common;
using Krake.Infrastructure.IO.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Krake.Cli.Features.FinanceData;

public sealed class FinanceDataImporterApp(
    IConfiguration config,
    ILogger logger,
    [FromKeyedServices("finance-data")] FinanceDataFileManager financeDataFileManager)
    : IImporterApplication
{
    public void Run(string[] args)
    {
        // Setup
        var directoryManager = financeDataFileManager.DirectoryManager;
        var inDirectory = directoryManager.In;
        var equitiesFile = inDirectory.EnumerateFiles()
            .First(x => x.Name.Contains(".csv", StringComparison.OrdinalIgnoreCase) &&
                        x.Name.Contains(config["Apps:FinanceData:EquitiesFile"]!));

        var fileReaderOptions = new FileReaderOptions
        {
            FileInfo = new FileInfo(equitiesFile.FullName),
            Delimiter = ',',
            HasHeaders = true,
            SkipLines = 0,
            Encoding = Encoding.UTF8
        };

        var rawData = financeDataFileManager.Read(fileReaderOptions).Match(
            error => throw new Exception(error.Message),
            data => data);

        Console.WriteLine();
    }
}