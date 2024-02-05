using HtmlAgilityPack;

namespace Krake.Cli.WebScrapers;

using GicsLookup = Dictionary<string, string>;

internal static class GicsWebScraper
{
    public static GicsLookup ScrapeFromWikipedia(Func<Uri, HtmlDocument> htmlParser)
    {
        var uri = new Uri("https://en.wikipedia.org/wiki/Global_Industry_Classification_Standard");
        var htmlDocument = htmlParser(uri);

        var wikiTableNodes = htmlDocument.DocumentNode.SelectNodes("//table[@class='wikitable']");
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

        var gics = new GicsLookup();
        for (var irow = 1; irow < rows.Count; irow++)
        {
            var cells = rows[irow].SelectNodes("//td");

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