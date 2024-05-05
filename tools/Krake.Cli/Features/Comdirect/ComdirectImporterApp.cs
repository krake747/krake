using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Krake.Cli.Features.Common;
using Krake.Core.Builders;
using Krake.Core.Functional;
using Krake.Infrastructure.Email.Builders;
using Krake.Infrastructure.Email.Models;
using Krake.Infrastructure.Email.Services;
using Krake.Infrastructure.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using Serilog;
using Spectre.Console;
using static Krake.Core.Monads.ErrorBase;

namespace Krake.Cli.Features.Comdirect;

public sealed class ComdirectImporterApp(
    IConfiguration config,
    ILogger logger,
    IAnsiConsole console,
    IMailKitEmailService emailService,
    EmailTemplate emailTemplate,
    [FromKeyedServices("comdirect")] ComdirectFileManager comdirectFileManager)
    : IImporterApplication
{
    public void Run(string[] args)
    {
        // Setup
        var directoryManager = comdirectFileManager.DirectoryManager;
        var inDirectory = directoryManager.In;
        if (inDirectory.EnumerateFiles().Any() is false)
        {
            logger.Warning("No files in the Inbound folder");
            return;
        }

        var allFiles = inDirectory.GetFiles();

        var bankAccountsFile = allFiles
            .FirstOrDefault(x =>
                CheckTxtExtension(x.Name) && x.Name.Contains(config["Apps:Comdirect:BankAccountsFile"]!));

        var portfolioFile = allFiles
            .First(x => CheckCsvExtension(x.Name) && x.Name.Contains(config["Apps:Comdirect:PortfolioFile"]!));

        var positionDate = Path.GetFileNameWithoutExtension(portfolioFile.Name)
            .Split('_')[^2]
            .Pipe(x => DateOnly.ParseExact(x, "yyyyMMdd"));

        // Read BankAccounts data
        var rawBankAccountData = RawBankAccountData(bankAccountsFile);

        // Read Portfolio data
        var portfolioFileReaderOptions = new FileReaderOptions
        {
            FileInfo = new FileInfo(portfolioFile.FullName),
            Delimiter = ';',
            HasHeaders = true,
            SkipLines = 5,
            Encoding = Encoding.Latin1
        };

        var rawData = comdirectFileManager.Read(portfolioFileReaderOptions).Match(
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
        var bankAccountsPortfolioData = rawBankAccountData
            .Select(x => ComdirectPortfolioDataExtensions.MapBankAccountsToPortfolioData(x, portfolioOverrides))
            .ToArray();

        var portfolioData = cleanData.Select(ComdirectPortfolioData.Create)
            .Select(x => x.MapToPortfolioData(portfolioOverrides, defaultFormatProvider))
            .ToArray();

        // Temp positions only
        var positionsPortfolioData = portfolioData.ToArray();

        var positionsDataLookup = positionsPortfolioData
            .ToLookup(k => (k.Isin, k.LocalCurrency), v => v);

        if (positionsDataLookup.All(x => x.Key.LocalCurrency is baseCurrency) is false)
        {
            throw new Exception("Requires Fx Rates");
        }

        // Temp Use Case #1: Aggregate data
        var aggregatedPositionsPortfolioData = positionsDataLookup.Select(g =>
        {
            var posDate = g.First().PositionDate;
            var securityName = g.First().SecurityName;
            var totalShares = g.Sum(x => x.NumberOfShares);
            var totalValue = g.Sum(x => x.BaseReportedValue);
            var totalCostValue = g.Sum(x => x.BaseCostValue);
            var performance = totalValue / totalCostValue - 1;
            return new Position(posDate, securityName, g.Key.Isin, g.Key.LocalCurrency, totalValue, totalShares,
                totalCostValue, performance);
        }).ToArray();

        var allPortfolioData = portfolioData.Concat(bankAccountsPortfolioData).ToArray();

        var portfolioDataLookup = allPortfolioData
            .ToLookup(k => (k.Isin, k.LocalCurrency), v => v);

        if (portfolioDataLookup.All(x => x.Key.LocalCurrency is baseCurrency) is false)
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
            var performance = totalValue / totalCostValue - 1;
            return new Position(posDate, securityName, g.Key.Isin, g.Key.LocalCurrency, totalValue, totalShares,
                totalCostValue, performance);
        }).ToArray();

        // Use Case #2: Calculate Total Portfolio Value
        var longTermPositions = static (Position p) => p.Name.Contains("Pension") || p.Name.Contains("Loans");

        var longTermPortfolioData = aggregatedPortfolioData
            .Where(longTermPositions)
            .ToArray();

        var shortTermPortfolioData = aggregatedPortfolioData
            .Where(x => longTermPositions(x) is false)
            .ToArray();

        var nav = aggregatedPortfolioData.Sum(x => x.TotalValue);
        var positionsNav = aggregatedPositionsPortfolioData.Sum(x => x.TotalValue);
        var navShortTerm = shortTermPortfolioData.Sum(x => x.TotalValue);
        var navLongTerm = longTermPortfolioData.Sum(x => x.TotalValue);

        logger.Debug("Nav is {Nav:F2} {BaseCurrency}", nav, baseCurrency);
        foreach (var position in aggregatedPortfolioData)
        {
            logger.Debug("Position: {Position}", position);
        }

        console.WriteLine($"Nav is {nav:F2} {baseCurrency}");
        RenderPortfolioPositions(console, aggregatedPortfolioData);

        // Store Processed data in file
        var exportData = comdirectFileManager.Write(allPortfolioData).Match(
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

        if (bankAccountsFile is not null)
        {
            _ = directoryManager.MoveFileToProcessed(bankAccountsFile, $"{positionDate:O}_processed_in.zip")
                .DoIfError(() => directoryManager.MoveFileToFailed(bankAccountsFile, $"{positionDate:O}_failed_in.zip"))
                .DoIfError(() =>
                    throw new Exception(Error.Failure().WithAttemptedValue(bankAccountsFile.Name).ToString()))
                .AsValue;
        }

        // Data archiving In Directory
        _ = directoryManager.ZipInDirectoryToArchive($"{positionDate:O}_reports_in.zip")
            .DoIfError(() => throw new Exception(Error.Unexpected("Unable to create the zip file").ToString()))
            .AsValue;

        // Data archiving Out Directory
        var outZipReportsFileInfo = directoryManager.ZipOutDirectoryToArchive($"{positionDate:O}_reports_out.zip")
            .DoIfError(() => throw new Exception(Error.Unexpected("Unable to create the zip file").ToString()))
            .AsValue;

        // Optional - Create Feedback Portfolio Report
        const bool sendFeedbackEmail = true;
        if (sendFeedbackEmail)
        {
            var sendEmailCommand = new SendEmailCommand
            {
                To = { emailTemplate.To ?? string.Empty },
                From = emailTemplate.From ?? string.Empty,
                DisplaySenderName = emailTemplate.DisplayName ?? "Krake Report Team",
                Subject = $"Krake Daily Report - {positionDate:O}",
                Body = PortfolioReportHtmlBody(nav, positionsNav, navShortTerm, navLongTerm,
                    baseCurrency, positionDate,
                    aggregatedPositionsPortfolioData, shortTermPortfolioData, longTermPortfolioData),
                BodyFormat = EmailBodyFormat.Html,
                Attachments = { outZipReportsFileInfo.FullName }
            };

            _ = CreateReportEmailMessage(sendEmailCommand)
                .Pipe(emailService.Send)
                .Tap(sent => logger.Information("Email sent: {Sent}", sent));
        }

        // Post Cleanup - Delete Files
        inDirectory.EnumerateFiles()
            .Concat(outDirectory.EnumerateFiles())
            .ToList()
            .ForEach(x => x.Delete());
    }

    private List<Dictionary<string, string>> RawBankAccountData(FileInfo? bankAccountsFile)
    {
        if (bankAccountsFile is null)
        {
            return [];
        }

        var bankAccountsReaderOptions = new FileReaderOptions
        {
            FileInfo = new FileInfo(bankAccountsFile.FullName),
            Delimiter = ';',
            HasHeaders = true,
            SkipLines = 0,
            Encoding = Encoding.UTF8
        };

        var rawBankAccountData = comdirectFileManager.Read(bankAccountsReaderOptions).Match(
            error => throw new Exception(error.Message),
            data => data);
        return rawBankAccountData;
    }

    private static void RenderPortfolioPositions(IAnsiConsole console, IEnumerable<Position> positions)
    {
        var table = new Table();

        table.AddColumn(new TableColumn(nameof(Position.Name)));
        table.AddColumn(new TableColumn(nameof(Position.Isin)));
        table.AddColumn(new TableColumn("Local Ccy"));
        table.AddColumn(new TableColumn("Shares").RightAligned());
        table.AddColumn(new TableColumn("Local Price").RightAligned());
        table.AddColumn(new TableColumn(nameof(Position.TotalValue)).RightAligned());
        table.AddColumn(new TableColumn(nameof(Position.TotalCostValue)).RightAligned());
        table.AddColumn(new TableColumn(nameof(Position.Performance)).RightAligned());

        foreach (var position in positions)
        {
            table.AddRow([
                position.Name,
                position.Isin,
                position.LocalCurrency,
                $"{position.NumberOfShares:F2}",
                $"{position.TotalValue / position.NumberOfShares}",
                $"{position.TotalValue:F2}",
                $"{position.TotalCostValue:F2}",
                $"{position.Performance:P2}"
            ]);
        }

        console.Write(table);
    }

    private static FileInfo CreateFileInfo(FileSystemInfo outDirectory, string fileName) =>
        new(Path.Combine(outDirectory.FullName, fileName));

    private static string UnescapeChars(string k) =>
        Regex.Unescape(k.AsSpan(1, k.Length - 2).ToString());

    private static bool CheckCsvExtension(string fileName) =>
        fileName.Contains(".csv", StringComparison.OrdinalIgnoreCase);

    private static bool CheckTxtExtension(string fileName) =>
        fileName.Contains(".txt", StringComparison.OrdinalIgnoreCase);

    private static MimeMessage CreateReportEmailMessage(SendEmailCommand sendEmailCommand) =>
        new MimeMessageBuilder()
            .From(sendEmailCommand.From)
            .To(sendEmailCommand.To)
            .Subject(sendEmailCommand.Subject)
            .BodyWithAttachments(sendEmailCommand.Body, sendEmailCommand.BodyFormat, sendEmailCommand.Attachments)
            .Build()
            .Match(error => throw new InvalidOperationException(error.ToString()), email => email);

    private static string PortfolioReportHtmlBody(
        decimal nav, decimal positionsNav, decimal shortTermNav, decimal longTermNav,
        string baseCurrency, DateOnly positionDate,
        IReadOnlyList<Position> positionsPortfolioData, IReadOnlyList<Position> shortTermPortfolioData,
        IReadOnlyList<Position> longTermPortfolioData)
    {
        var banksData = shortTermPortfolioData.Where(x =>
            x.Name.Contains("BCEE") || x.Name.Contains("BIL") || x.Name.Contains("Revolut")).ToArray();
        var banks = banksData.Sum(x => x.TotalValue);
        /* lang=html */
        return $$"""
                 <html lang="en">
                 <head>
                     <title>Krake Portfolio Reports</title>
                     <style>
                         table {
                           border-collapse: collapse;
                         }
                         th, td {
                           text-align: left;
                           padding: 6px;
                         }
                         tr {
                           border-bottom: 1px solid black;
                         }
                         .right {
                           text-align: right;
                         }
                     </style>
                 </head>
                 <body>
                     <h1>Portfolio Report</h1>
                     <p>Total Nav is {{nav:F2}} {{baseCurrency}}</p>
                     <p>Position date is {{positionDate:O}}</p>
                     <hr/>
                     <p>Positions Nav is {{positionsNav:F2}} {{baseCurrency}}</p>
                     <table>
                        <tr>
                            <th>{{nameof(Position.Name)}}</th>
                            <th>{{nameof(Position.Isin)}}</th>
                            <th>{{nameof(Position.LocalCurrency)}}</th>
                            <th>{{nameof(Position.NumberOfShares)}}</th>
                            <th>LocalPrice</th>
                            <th>{{nameof(Position.TotalValue)}}</th>
                            <th>{{nameof(Position.TotalCostValue)}}</th>
                            <th>{{nameof(Position.Performance)}}</th>
                        </tr>
                        {{PortfolioSummaryTable(positionsPortfolioData)}}
                     </table>
                     <hr/>
                     <p>Cash Nav is {{banks:F2}} {{baseCurrency}} (SubTotal: {{shortTermNav:F2}} {{baseCurrency}})</p>
                     <table>
                        <tr>
                            <th>{{nameof(Position.Name)}}</th>
                            <th>{{nameof(Position.Isin)}}</th>
                            <th>{{nameof(Position.LocalCurrency)}}</th>
                            <th>{{nameof(Position.NumberOfShares)}}</th>
                            <th>LocalPrice</th>
                            <th>{{nameof(Position.TotalValue)}}</th>
                            <th>{{nameof(Position.TotalCostValue)}}</th>
                            <th>{{nameof(Position.Performance)}}</th>
                        </tr>
                        {{PortfolioSummaryTable(banksData)}}
                    </table>
                    <hr/>
                    <table>
                    <p>LongTerm Nav is {{longTermNav:F2}} {{baseCurrency}}</p>
                    <tr>
                        <th>{{nameof(Position.Name)}}</th>
                        <th>{{nameof(Position.Isin)}}</th>
                        <th>{{nameof(Position.LocalCurrency)}}</th>
                        <th>{{nameof(Position.NumberOfShares)}}</th>
                        <th>LocalPrice</th>
                        <th>{{nameof(Position.TotalValue)}}</th>
                        <th>{{nameof(Position.TotalCostValue)}}</th>
                        <th>{{nameof(Position.Performance)}}</th>
                    </tr>
                    {{PortfolioSummaryTable(longTermPortfolioData)}}
                 </table>
                 <hr/>
                 </body>
                 </html>
                 """;
    }

    private static StringBuilder PortfolioSummaryTable(IEnumerable<Position> aggregatedPortfolioData) =>
        aggregatedPortfolioData.Aggregate(new HtmlStringBuilder(), (sb, x) => sb
                .AppendLine("<tr>")
                .DataCell(x.Name)
                .DataCell(x.Isin)
                .DataCell(x.LocalCurrency)
                .DataCell($"{x.NumberOfShares:F2}", "right")
                .DataCell($"{x.TotalValue / x.NumberOfShares:F2}", "right")
                .DataCell($"{x.TotalValue:F2}", "right")
                .DataCell($"{x.TotalCostValue:F2}", "right")
                .DataCell($"{x.Performance:P2}", "right")
                .AppendLine("</tr>"))
            .Build();

    private sealed class SendEmailCommand
    {
        public List<string> To { get; } = [];
        public string From { get; set; } = string.Empty;
        public string DisplaySenderName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public EmailBodyFormat BodyFormat { get; set; }
        public List<string> Attachments { get; } = [];
    }

    private sealed class HtmlStringBuilder : IFluentFunctionalBuilder<StringBuilder, HtmlStringBuilder>
    {
        private const string Td = "<td>";
        private const string TdClose = "</td>";
        private readonly List<Func<StringBuilder, StringBuilder>> _actions = [];
        private readonly StringBuilder _sb = new();

        public IReadOnlyList<Func<StringBuilder, StringBuilder>> Actions => _actions.AsReadOnly();

        public StringBuilder Build() => _actions.Aggregate(_sb, (x, f) => f(x));

        public HtmlStringBuilder Do(Action<StringBuilder> action)
        {
            _actions.Add(x =>
            {
                action(x);
                return x;
            });
            return this;
        }

        public HtmlStringBuilder Append(string value) =>
            Do(x => _sb.Append(value));

        public HtmlStringBuilder AppendLine(string value) =>
            Do(x => _sb.AppendLine(value));

        public HtmlStringBuilder DataCell(string value, string? classes = null) =>
            Do(x => _sb.Append(classes is not null ? TdWithClass(classes) : Td).Append(value).AppendLine(TdClose));

        private static string TdWithClass(string classes) => $"<td class='{classes}'>";
    }
}