using System.Reflection;
using Krake.Cli.PDF;
using Microsoft.Extensions.Configuration;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

QuestPDF.Settings.License = LicenseType.Community;

var config = new ConfigurationBuilder()
    .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!)
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

var directoryManager = new DirectoryManager(config["Budget:RootDirectory"]!);

var file = directoryManager.In.EnumerateFiles($"*{config["Budget:BudgetFile"]}*").First();

var budgetData = File.ReadLines(file.FullName)
    .Skip(1)
    .Select(line =>
    {
        var values = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
        if (values is not [var category, var name, var type, var subType, var price, var period, var expectedValue])
        {
            throw new Exception(nameof(values));
        }

        var parsedPrice = Convert.ToDecimal(price);
        var parsedPeriod = Convert.ToInt32(period);
        var parsedExpectedValue = Convert.ToDecimal(expectedValue);
        return new BudgetData(category, name, type, subType, parsedPrice, parsedPeriod, parsedExpectedValue);
    })
    .ToArray();

// code in your main method
var document = Document.Create(container =>
{
    container.Page(page =>
    {
        page.Size(PageSizes.A4);
        page.Margin(2, Unit.Centimetre);
        page.PageColor(Colors.White);
        page.DefaultTextStyle(x => x.FontSize(12));

        page.Header()
            .Text("Hello PDF!")
            .SemiBold().FontSize(32).FontColor(Colors.Blue.Medium);

        page.Content()
            .PaddingVertical(1, Unit.Centimetre)
            .Column(x =>
            {
                x.Spacing(20);

                x.Item().Text(Placeholders.LoremIpsum());
                x.Item().Image(Placeholders.Image(200, 100));
            });

        page.Footer()
            .AlignCenter()
            .Text(x =>
            {
                x.Span("Page ");
                x.CurrentPageNumber();
            });
    });
});

document.ShowInPreviewer();

namespace Krake.Cli.PDF
{
    public sealed record BudgetData(
        string Category,
        string Name,
        string Type,
        string SubType,
        decimal Price,
        int Period,
        decimal ExpectedValue);
}

