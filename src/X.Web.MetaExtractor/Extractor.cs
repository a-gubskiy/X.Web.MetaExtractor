using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using JetBrains.Annotations;
using X.Web.MetaExtractor.ContentLoaders.HttpClient;
using X.Web.MetaExtractor.LanguageDetectors;

namespace X.Web.MetaExtractor;

/// <summary>
/// Implements the <see cref="IExtractor"/> interface to extract metadata from web pages.
/// Extracts various elements like title, description, keywords, images, and language from HTML content.
/// </summary>
/// <remarks>
/// This class provides multiple constructors for customization including specifying a default image,
/// custom page content loader, and language detector. It handles extraction of OpenGraph properties
/// and standard HTML metadata.
/// </remarks>
public class Extractor : IExtractor
{
    [PublicAPI]
    public int MaxDescriptionLength { get; set; } = 300;

    private readonly string _defaultImage;
    private readonly IPageContentLoader _pageContentLoader;
    private readonly ILanguageDetector _languageDetector;

    public Extractor()
        : this(string.Empty, new HttpClientPageContentLoader(), new LanguageDetector())
    {
    }

    public Extractor(string defaultImage)
        : this(defaultImage, new HttpClientPageContentLoader(), new LanguageDetector())
    {
    }

    public Extractor(string defaultImage, IPageContentLoader pageContentLoader, ILanguageDetector languageDetector)
    {
        _defaultImage = defaultImage;
        _languageDetector = languageDetector;
        _pageContentLoader = pageContentLoader;
    }

    /// <inheritdoc />
    public async Task<Metadata> Extract(Uri uri, CancellationToken cancellationToken)
    {
        var html = await _pageContentLoader.LoadPageContent(uri, cancellationToken);

        var document = CreateHtmlDocument(html);

        var title = ExtractTitle(document);
        var keywords = ExtractKeywords(document);
        var metatags = ExtractMetaTags(document);
        var description = ExtractDescription(document);
        var images = ExtractImages(document, _defaultImage);
        var language = _languageDetector.GetHtmlPageLanguage(html);

        return new Metadata
        {
            Raw = html,
            Url = uri.ToString(),

            Title = title,
            Keywords = keywords,
            MetaTags = metatags,
            Description = description,
            Images = images,
            Language = language
        };
    }

    private static string ExtractTitle(HtmlDocument document)
    {
        var title = ReadOpenGraphProperty(document, "og:title");

        if (string.IsNullOrWhiteSpace(title))
        {
            var node = document.DocumentNode.SelectSingleNode("//head/title");

            title = node != null ? HtmlDecode(node.InnerText) : string.Empty;
        }

        return title;
    }

    private static IReadOnlyCollection<string> ExtractKeywords(HtmlDocument document)
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

    private static IReadOnlyCollection<KeyValuePair<string, string>> ExtractMetaTags(HtmlDocument document)
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

    private static string ExtractDescription(HtmlDocument document)
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

    private static IReadOnlyCollection<string> ExtractImages(HtmlDocument document, string defaultImage)
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

        if (!images.Any() && !string.IsNullOrWhiteSpace(defaultImage))
        {
            return ImmutableList.Create(defaultImage);
        }

        return images;
    }

    private static string OneOf(string a, string b) => string.IsNullOrWhiteSpace(b) ? a : b;

    private static HtmlDocument CreateHtmlDocument(string html)
    {
        var document = new HtmlDocument();
        document.LoadHtml(html);

        return document;
    }

    private static string ReadOpenGraphProperty(HtmlDocument document, string name)
    {
        var node = document.DocumentNode.SelectSingleNode($"//meta[@property='{name}']");
        return HtmlDecode(node?.Attributes["content"]?.Value ?? string.Empty).Trim();
    }

    private static string HtmlDecode(string text)
    {
        var result = string.IsNullOrWhiteSpace(text) ? string.Empty : WebUtility.HtmlDecode(text);

        return result;
    }
}