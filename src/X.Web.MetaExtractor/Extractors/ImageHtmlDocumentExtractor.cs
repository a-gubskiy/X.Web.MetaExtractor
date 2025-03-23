using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using HtmlAgilityPack;

namespace X.Web.MetaExtractor.Extractors;

public class ImageHtmlDocumentExtractor : HtmlDocumentExtractor<IReadOnlyCollection<string>>
{
    private readonly string _defaultImage;

    public ImageHtmlDocumentExtractor(string defaultImage)
    {
        _defaultImage = defaultImage;
    }

    protected override IReadOnlyCollection<string> ExtractInternal(HtmlDocument document)
    {
        var image = ReadOpenGraphProperty(document, "og:image");

        if (!string.IsNullOrWhiteSpace(image))
        {
            //When image defined via Open Graph
            return ImmutableList.Create(image);
        }

        var images = document.DocumentNode
            .Descendants("img")
            .Select(e => e.GetAttributeValue("src", ""))
            .Where(src => !string.IsNullOrWhiteSpace(src))
            .ToImmutableList();

        if (!images.Any() && !string.IsNullOrWhiteSpace(_defaultImage))
        {
            return ImmutableList.Create(_defaultImage);
        }

        return images.ToImmutableList();
    }
}