using System.Xml;
using HtmlAgilityPack;

namespace Krake.Cli.WebScrapers;

public static class DocumentParsingService
{
    public static HtmlDocument ParseHtmlDocument(Uri uri)
    {
        var web = new HtmlWeb();
        var htmlDocument = web.Load(uri);
        return htmlDocument;
    }

    public static async Task<HtmlDocument> ParseHtmlDocumentAsync(Uri uri)
    {
        var web = new HtmlWeb();
        var htmlDocument = await web.LoadFromWebAsync(uri.AbsoluteUri);
        return htmlDocument;
    }

    public static XmlDocument ParseXmlDocument(Uri uri)
    {
        var xmlDocument = new XmlDocument();
        xmlDocument.Load(uri.AbsoluteUri);
        return xmlDocument;
    }
}