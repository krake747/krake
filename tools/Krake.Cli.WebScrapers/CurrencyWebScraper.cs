using System.Xml;
using HtmlAgilityPack;

namespace Krake.Cli.WebScrapers;

using CurrenciesLookup = Dictionary<string, string>;

public static class CurrencyWebScraper
{
    public static async Task<CurrenciesLookup> ScrapeFromSixGroup(Func<Uri, Task<HtmlDocument>> htmlParser,
        Func<Uri, XmlDocument> xmlParser)
    {
        const string baseUri = "https://www.six-group.com/";
        var websiteUri = new Uri($"{baseUri}/en/products-services/financial-information/data-standards.html");
        var htmlDocument = await htmlParser(websiteUri);

        var hyperLinkNodes = htmlDocument.DocumentNode.SelectNodes("//a[@href]");

        var xmlLink = hyperLinkNodes
            .Where(x => x.InnerText.Contains("List One (XML)", StringComparison.OrdinalIgnoreCase))
            .SelectMany(x => x.Attributes)
            .Single(x => x.Value.Contains(".xml", StringComparison.OrdinalIgnoreCase))
            .Value;

        var xmlUri = new Uri(xmlLink);
        var xmlDocument = xmlParser(xmlUri);

        var root = xmlDocument.DocumentElement;
        var childNodes = root?.ChildNodes[0]?.ChildNodes;
        if (childNodes is null)
        {
            return [];
        }

        var currencies = new CurrenciesLookup();
        foreach (XmlNode node in childNodes)
        {
            var code = node["Ccy"]?.InnerXml ?? string.Empty;
            var name = node["CcyNm"]?.InnerXml ?? string.Empty;
            _ = currencies.TryAdd(code, name);
        }

        return currencies;
    }
}