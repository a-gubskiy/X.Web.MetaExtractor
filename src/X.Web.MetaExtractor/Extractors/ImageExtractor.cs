using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using HtmlAgilityPack;

namespace X.Web.MetaExtractor.Extractors;

public class ImageExtractor : ExtractorBase
{
    private readonly string _defaultImage;

    public ImageExtractor(string defaultImage)
    {
        _defaultImage = defaultImage;
    }

    public IReadOnlyCollection<string> ExtractImages(HtmlDocument document)
    {
        var image = ReadOpenGraphProperty(document, "og:image");

        if (!string.IsNullOrWhiteSpace(image))
        {
            //When image defined via Open Graph
            return ImmutableList.Create(image);
        }

        var images = document.DocumentNode
            .Descendants("img")
            .Select(e => e.GetAttributeValue("src", null))
            .Where(src => !string.IsNullOrWhiteSpace(src))
            .ToImmutableList();

        if (!images.Any() && !string.IsNullOrWhiteSpace(_defaultImage))
        {
            return ImmutableList.Create(_defaultImage);
        }

        return images;
    }
}