using HtmlAgilityPack;

namespace X.Web.MetaExtractor.Extractors;

public class TitleExtractor : ExtractorBase
{
    public string ExtractTitle(HtmlDocument document)
    {
        var title = ReadOpenGraphProperty(document, "og:title");

        if (string.IsNullOrWhiteSpace(title))
        {
            var node = document.DocumentNode.SelectSingleNode("//head/title");

            title = node != null ? HtmlDecode(node.InnerText) : string.Empty;
        }

        return title;
    }
}