using System.Net;
using HtmlAgilityPack;

namespace X.Web.MetaExtractor.Extractors;

public abstract class ExtractorBase
{
    protected string ReadOpenGraphProperty(HtmlDocument document, string name)
    {
        var node = document.DocumentNode.SelectSingleNode($"//meta[@property='{name}']");
        var result = HtmlDecode(node?.Attributes["content"]?.Value ?? string.Empty).Trim();

        return result;
    }

    protected string HtmlDecode(string text)
    {
        var result = string.IsNullOrWhiteSpace(text) ? string.Empty : WebUtility.HtmlDecode(text);

        return result;
    }
}