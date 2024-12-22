using System.Xml;
using HtmlAgilityPack;
using Tabula;
using Tabula.Detectors;
using Tabula.Extractors;
using UglyToad.PdfPig;

namespace Krake.Cli.WebScrapers;

using MicLookup = Dictionary<string, string>;
using MicMarketCategories = Dictionary<string, string>;

internal static class MicWebScraper
{
    public static async Task<MicLookup> ScrapeFromIsoWebsite(
        Func<HttpClient> httpFactory,
        Func<Uri, Task<HtmlDocument>> htmlParser,
        Func<Uri, XmlDocument> xmlParser)
    {
        const string baseUri = "https://www.iso20022.org";

        var htmlDocument = await htmlParser(new Uri($"{baseUri}/market-identifier-codes"));

        var hyperLinkNodes = htmlDocument.DocumentNode.SelectNodes("//a[@href]");

        var pdfLink = hyperLinkNodes
            .Where(x => x.InnerText.Contains("Download the FAQ", StringComparison.OrdinalIgnoreCase))
            .SelectMany(x => x.Attributes)
            .Single(x => x.Value.Contains(".pdf", StringComparison.OrdinalIgnoreCase))
            .Value;

        var micMarketCodes = ScrapeMicMarketCategories(httpFactory, new Uri($"{baseUri}/{pdfLink}"));

        var xmlLink = hyperLinkNodes
            .Where(x => x.InnerText is "Download")
            .SelectMany(x => x.Attributes)
            .Single(x => x.Value.Contains(".xml", StringComparison.OrdinalIgnoreCase))
            .Value;

        var uri = new Uri($"{baseUri}{xmlLink}");
        var xmlDocument = xmlParser(uri);

        var root = xmlDocument.DocumentElement;
        var childNodes = root?.ChildNodes;
        if (childNodes is null)
        {
            return [];
        }

        var mics = new MicLookup();
        var xmlNodes = childNodes
            .Cast<XmlNode>()
            .Where(x => x["STATUS"]?.InnerXml is "ACTIVE"
                        && x["MARKET_x0020_CATEGORY_x0020_CODE"]?.InnerXml == micMarketCodes["Regulated Market"]);

        foreach (var node in xmlNodes)
        {
            var mic = node["MIC"]?.InnerXml ?? string.Empty;
            var micName = node["MARKET_x0020_NAME-INSTITUTION_x0020_DESCRIPTION"]?.InnerXml ?? string.Empty;
            _ = mics.TryAdd(mic, micName);
        }

        return mics;
    }

    private static MicMarketCategories ScrapeMicMarketCategories(Func<HttpClient> httpFactory, Uri uri)
    {
        using var http = httpFactory();
        var fileContent = http.GetByteArrayAsync(uri.AbsoluteUri)
            .GetAwaiter()
            .GetResult();

        using var document = PdfDocument.Open(fileContent);
        var page = ObjectExtractor.ExtractPage(document, 5);

        var detector = new SimpleNurminenDetectionAlgorithm();
        var regions = detector.Detect(page);

        var extractionAlgorithm = new BasicExtractionAlgorithm();
        var tables = extractionAlgorithm.Extract(page.GetArea(regions[0].BoundingBox));
        var table = tables[0];

        var micMarketCodes = new MicMarketCategories();
        for (var irow = 1; irow < table.RowCount; irow++)
        {
            var category = table.Rows[irow][0].GetText();
            var code = table.Rows[irow][1].GetText();
            micMarketCodes.Add(category, code);
        }

        return micMarketCodes;
    }
}