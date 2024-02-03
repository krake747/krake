using Krake.Cli.WebScrapers;
using static Krake.Cli.WebScrapers.HtmlParserService;

var gicsMap = GicsWebScraper.ScrapeFromWikipedia(ParseHtmlDocument);
var trbcMap = TrbcWebScraper.ScrapeFromWikipedia(ParseHtmlDocument);

Console.WriteLine($"Scraped GICS Count: {gicsMap.Count}");
Console.WriteLine($"Scraped TRBC Count: {trbcMap.Count}");