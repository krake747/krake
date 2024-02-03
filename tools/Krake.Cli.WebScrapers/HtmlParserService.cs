using HtmlAgilityPack;

namespace Krake.Cli.WebScrapers;

public static class HtmlParserService
{
    public static HtmlDocument ParseHtmlDocument(string url)
    {
        var web = new HtmlWeb();
        var doc = web.Load(url);
        return doc;
    }
}