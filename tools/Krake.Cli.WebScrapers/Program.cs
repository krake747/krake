using Krake.Cli.WebScrapers;
using static Krake.Cli.WebScrapers.HtmlParserService;

var gicsMap = GicsWebScraper.ScrapeFromWikipedia(ParseHtmlDocument);
Console.WriteLine();