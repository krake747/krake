﻿using System.Diagnostics;
using Krake.Cli.WebScrapers;
using static Krake.Cli.WebScrapers.DocumentParsingService;

var sw = Stopwatch.StartNew();

var httpFactory = () => new HttpClient();

var gicsTask = GicsWebScraper.ScrapeFromWikipedia(ParseHtmlDocumentAsync);
var trbcTask = TrbcWebScraper.ScrapeFromWikipedia(ParseHtmlDocumentAsync);
var micsTask = MicWebScraper.ScrapeFromIsoWebsite(httpFactory, ParseHtmlDocumentAsync, ParseXmlDocument);
var ctryTask = CountryWebScraper.ScrapeFromGitHub(httpFactory);
var ccysTask = CurrencyWebScraper.ScrapeFromSixGroup(ParseHtmlDocumentAsync, ParseXmlDocument);

var results = await Task.WhenAll(gicsTask, trbcTask, micsTask, ctryTask, ccysTask);

Console.WriteLine($"Scraped GICS Count: {results[0].Count}");
Console.WriteLine($"Scraped TRBC Count: {results[1].Count}");
Console.WriteLine($"Scraped MIC Count: {results[2].Count}");
Console.WriteLine($"Scraped Countries Count: {results[3].Count}");
Console.WriteLine($"Scraped Currencies Count: {results[4].Count}");

Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds}ms");
sw.Stop();