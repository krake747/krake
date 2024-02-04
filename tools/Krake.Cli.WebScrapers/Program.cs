using System.Diagnostics;
using Krake.Cli.WebScrapers;
using static Krake.Cli.WebScrapers.DocumentParsingService;

var sw = Stopwatch.StartNew();

var gicsTask = Task.Run(() => GicsWebScraper.ScrapeFromWikipedia(ParseHtmlDocument));
var trbcTask = Task.Run(() => TrbcWebScraper.ScrapeFromWikipedia(ParseHtmlDocument));
var micsTask = Task.Run(() =>
    MicWebScraper.ScrapeFromIsoWebsite(() => new HttpClient(), ParseHtmlDocument, ParseXmlDocument));

var results = await Task.WhenAll(gicsTask, trbcTask, micsTask);

Console.WriteLine($"Scraped GICS Count: {results[0].Count}");
Console.WriteLine($"Scraped TRBC Count: {results[1].Count}");
Console.WriteLine($"Scraped MIC Count: {results[2].Count}");

Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds}ms");
sw.Stop();