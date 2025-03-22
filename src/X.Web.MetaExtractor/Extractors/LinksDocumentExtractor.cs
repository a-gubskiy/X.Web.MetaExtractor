using System.Collections.Generic;
using HtmlAgilityPack;
using X.Web.MetaExtractor.Models;

namespace X.Web.MetaExtractor.Extractors;

public class LinksDocumentExtractor : HtmlDocumentExtractor<IReadOnlyCollection<Link>>
{
    protected override IReadOnlyCollection<Link> ExtractInternal(HtmlDocument document)
    {
        var linkNodes = document.DocumentNode.SelectNodes("//a[@href]");

        if (linkNodes == null)
        {
            return new List<Link>();
        }

        var links = new List<Link>();

        foreach (var linkNode in linkNodes)
        {
            var href = linkNode.Attributes["href"]?.Value;

            if (string.IsNullOrWhiteSpace(href))
            {
                continue;
            }

            var url = HtmlDecode(href);
            var text = HtmlDecode(linkNode.InnerText);

            links.Add(new Link
            {
                Title = text,
                Value = url
            });
        }

        return links;
    }
}