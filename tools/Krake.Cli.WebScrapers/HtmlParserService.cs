using HtmlAgilityPack;

namespace Krake.Cli.WebScrapers;

public static class HtmlParserService
{
    public static HtmlDocument ParseHtmlDocument(Uri uri)
    {
        var web = new HtmlWeb();
        var doc = web.Load(uri);
        return doc;
    }
}