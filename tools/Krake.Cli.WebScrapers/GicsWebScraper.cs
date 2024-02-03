using HtmlAgilityPack;

namespace Krake.Cli.WebScrapers;

using GicsMap = Dictionary<string, string>;

public static class GicsWebScraper
{
    public static GicsMap ScrapeFromWikipedia(Func<string, HtmlDocument> htmlParser)
    {
        const string url = "https://en.wikipedia.org/wiki/Global_Industry_Classification_Standard";
        var doc = htmlParser(url);

        var wikiTableNodes = doc.DocumentNode.SelectNodes("//table[@class='wikitable']");
        if (wikiTableNodes.Count > 1)
        {
            return [];
        }

        var table = wikiTableNodes.Single();
        var rows = table.SelectNodes(".//tr");
        if (rows is null)
        {
            return [];
        }

        var gics = new GicsMap();
        for (var i = 1; i < rows.Count; i++)
        {
            var cells = rows[i].SelectNodes("//td");

            for (var c = 0; c < cells.Count; c++)
            {
                if (c % 2 is not 0)
                {
                    continue;
                }

                var node = cells[c];
                var code = node.InnerText.Trim();
                var description = node.NextSibling.NextSibling.InnerText.Trim().Replace("&amp;", "&");
                _ = gics.TryAdd(code, description);
            }
        }

        return gics;
    }
}