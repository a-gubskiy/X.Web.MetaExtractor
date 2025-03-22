using HtmlAgilityPack;

namespace X.Web.MetaExtractor.Extractors;

public class DescriptionHtmlDocumentExtractor : HtmlDocumentExtractor<string>
{
    public override string Extract(HtmlDocument document)
    {
        var description = ReadOpenGraphProperty(document, "og:description");

        if (!string.IsNullOrWhiteSpace(description))
        {
            return description;
        }

        var node = document.DocumentNode.SelectSingleNode("//meta[@name='description']");

        if (node == null)
        {
            return string.Empty;
        }

        var content = node.Attributes["content"]?.Value;

        return HtmlDecode(content ?? string.Empty);
    }
}