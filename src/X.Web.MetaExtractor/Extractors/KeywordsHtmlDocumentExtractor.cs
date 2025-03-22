using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using HtmlAgilityPack;

namespace X.Web.MetaExtractor.Extractors;

public class KeywordsHtmlDocumentExtractor : HtmlDocumentExtractor<IReadOnlyCollection<string>>
{
    public override IReadOnlyCollection<string> Extract(HtmlDocument document)
    {
        var node = document.DocumentNode.SelectSingleNode("//meta[@name='keywords']");
        var value = string.Empty;

        if (node != null)
        {
            var content = node.Attributes["content"]?.Value ?? string.Empty;
            value = HtmlDecode(content);
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            return ImmutableArray<string>.Empty;
        }

        var result = value
            .Split(',')
            .Select(o => o?.Trim())
            .Where(o => !string.IsNullOrWhiteSpace(o))
            .Select(o => o!)
            .ToImmutableList();

        return result;
    }
}