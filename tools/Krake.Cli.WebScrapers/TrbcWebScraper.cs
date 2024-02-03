using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Krake.Cli.WebScrapers;

using TrbcLookup = Dictionary<string, string>;

internal static partial class TrbcWebScraper
{
    public static TrbcLookup ScrapeFromWikipedia(Func<Uri, HtmlDocument> htmlParser)
    {
        var uri = new Uri("https://en.wikipedia.org/wiki/The_Refinitiv_Business_Classification");
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

        var trbc = new TrbcLookup();
        for (var irow = 1; irow < rows.Count; irow++)
        {
            var cells = rows[irow].InnerHtml.Replace("&amp;", "&").Replace("\n", "");

            var captures = MyTrbcRegex().Match(cells)
                .Groups[1].Captures
                .Select(x => x.Value)
                .Where(x => string.IsNullOrEmpty(x) is false)
                .ToArray();

            if (captures is not [.. var descriptions, var code])
            {
                continue;
            }

            var level = 10;
            var stack = new Stack<string>(descriptions);
            while (stack.Count > 0)
            {
                var description = stack.Pop();
                _ = trbc.TryAdd(code[..level], description);
                level -= 2;
            }
        }

        return trbc;
    }

    [GeneratedRegex("(?:<td>(.*?)</td>){6}")]
    private static partial Regex MyTrbcRegex();
}