using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using HtmlAgilityPack;

namespace X.Web.MetaExtractor.Extractors;

public class MetaHtmlDocumentExtractor : HtmlDocumentExtractor<IReadOnlyCollection<KeyValuePair<string, string>>>
{
    protected override IReadOnlyCollection<KeyValuePair<string, string>> ExtractInternal(HtmlDocument document)
    {
        var result = new List<KeyValuePair<string, string>>();

        var list = document.DocumentNode?.SelectNodes("//meta");

        if (list == null || !list.Any())
        {
            return ImmutableArray<KeyValuePair<string, string>>.Empty;
        }

        foreach (var node in list)
        {
            var value = node.GetAttributeValue("content", "");
            var key1 = node.GetAttributeValue("property", "");
            var key2 = node.GetAttributeValue("name", "");

            if (string.IsNullOrWhiteSpace(key1) && string.IsNullOrWhiteSpace(key2))
            {
                continue;
            }

            result.Add(new KeyValuePair<string, string>(OneOf(key1, key2), value));
        }

        return result.ToImmutableList();
    }

    private static string OneOf(string a, string b) => string.IsNullOrWhiteSpace(b) ? a : b;
}