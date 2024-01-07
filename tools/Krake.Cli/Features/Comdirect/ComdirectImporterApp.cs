using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Cocona;
using Krake.Application.Portfolios;
using Krake.Cli.Features.Common;
using Krake.Core;
using Krake.Core.Builders;
using Krake.Core.Functional;
using Krake.Infrastructure.Email.Builders;
using Krake.Infrastructure.Email.Models;
using Krake.Infrastructure.Email.Services;
using Krake.Infrastructure.IO.Models;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Serilog;

namespace Krake.Cli.Features.Comdirect;

public sealed class ComdirectImporterApp(
    [FromService] IConfiguration config,
    [FromService] ILogger logger,
    [FromService] DirectoryManager directoryManager,
    [FromService] ComdirectFileManager comdirectFileManager,
    [FromService] IMailKitEmailService emailService,
    [FromService] EmailTemplate emailTemplate)
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
                Body = PortfolioReportHtmlBody(nav, baseCurrency, positionDate, aggregatedPortfolioData),
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

    private static FileInfo CreateFileInfo(FileSystemInfo outDirectory, string fileName) =>
        new(Path.Combine(outDirectory.FullName, fileName));

    private static string UnescapeChars(string k) =>
        Regex.Unescape(k.AsSpan(1, k.Length - 2).ToString());

    private static bool CheckCsvExtension(string fileName) =>
        fileName.Contains(".csv", StringComparison.OrdinalIgnoreCase);

    private static MimeMessage CreateReportEmailMessage(SendEmailCommand sendEmailCommand) =>
        new MimeMessageBuilder()
            .From(sendEmailCommand.From)
            .To(sendEmailCommand.To)
            .Subject(sendEmailCommand.Subject)
            .BodyWithAttachments(sendEmailCommand.Body, sendEmailCommand.BodyFormat, sendEmailCommand.Attachments)
            .Build()
            .Match(error => throw new InvalidOperationException(error.ToString()), email => email);

    private static string PortfolioReportHtmlBody(decimal nav, string baseCurrency, DateOnly positionDate,
        IEnumerable<Position> aggregatedPortfolioData) => /* lang=html */
        $$"""
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
                    padding-right: 33%;
                  }
              </style>
          </head>
          <body>
              <h1>Portfolio Report</h1>
              <p>Nav is {{nav:F2}} {{baseCurrency}}</p>
              <p>Position date is {{positionDate:O}}</p>
              <table>
                  <tr>
                      <th>{{nameof(Position.Name)}}</th>
                      <th>{{nameof(Position.Isin)}}</th>
                      <th>{{nameof(Position.LocalCurrency)}}</th>
                      <th>{{nameof(Position.NumberOfShares)}}</th>
                      <th>LocalPrice</th>
                      <th>{{nameof(Position.TotalValue)}}</th>
                      <th>{{nameof(Position.TotalCostValue)}}</th>
                  </tr>
                  {{PortfolioSummaryTable(aggregatedPortfolioData)}}
              </table>
          </body>
          </html>
          """;

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
        private readonly List<Func<StringBuilder, StringBuilder>> _actions = new();
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