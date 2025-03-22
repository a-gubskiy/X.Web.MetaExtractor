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
using X.Web.MetaExtractor.Extractors;
using X.Web.MetaExtractor.LanguageDetectors;
using X.Web.MetaExtractor.Models;

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

    private readonly IContentLoader _contentLoader;
    private readonly ILanguageDetector _languageDetector;
    private readonly TitleHtmlDocumentExtractor _titleHtmlDocumentExtractor;
    private readonly KeywordsHtmlDocumentExtractor _keywordsHtmlDocumentExtractor;
    private readonly MetaHtmlDocumentExtractor _metaHtmlDocumentExtractor;
    private readonly DescriptionHtmlDocumentExtractor _descriptionHtmlDocumentExtractor;
    private readonly ImageHtmlDocumentExtractor _imageHtmlDocumentExtractor;
    private readonly LinksDocumentExtractor _linksDocumentExtractor;
    

    public Extractor()
        : this(string.Empty, new HttpClientContentLoader(), new LanguageDetector())
    {
    }

    public Extractor(string defaultImage)
        : this(defaultImage, new HttpClientContentLoader(), new LanguageDetector())
    {
    }

    public Extractor(string defaultImage, IContentLoader contentLoader, ILanguageDetector languageDetector)
    {
        _languageDetector = languageDetector;
        _contentLoader = contentLoader;
        _linksDocumentExtractor = new LinksDocumentExtractor();
        _titleHtmlDocumentExtractor = new TitleHtmlDocumentExtractor();
        _keywordsHtmlDocumentExtractor = new KeywordsHtmlDocumentExtractor();
        _metaHtmlDocumentExtractor = new MetaHtmlDocumentExtractor();
        _descriptionHtmlDocumentExtractor = new DescriptionHtmlDocumentExtractor();
        _imageHtmlDocumentExtractor = new ImageHtmlDocumentExtractor(defaultImage);
    }

    /// <inheritdoc />
    public async Task<WebPage> Extract(Uri uri, CancellationToken cancellationToken)
    {
        var html = await _contentLoader.Load(uri, cancellationToken);

        var document = CreateHtmlDocument(html);

        var title = _titleHtmlDocumentExtractor.Extract(document);
        var keywords = _keywordsHtmlDocumentExtractor.Extract(document);
        var meta = _metaHtmlDocumentExtractor.Extract(document);
        var description = _descriptionHtmlDocumentExtractor.Extract(document);
        var images = _imageHtmlDocumentExtractor.Extract(document);
        var links = _linksDocumentExtractor.Extract(document);
        var language = _languageDetector.GetHtmlPageLanguage(html);

        return new WebPage
        {
            Source = new Source
            {
                Raw = html,
                Url = uri,
            },
            Links = links ?? ImmutableList<Link>.Empty,
            Title = title ?? string.Empty,
            Keywords = keywords ?? ImmutableList<string>.Empty,
            Metadata = meta ?? ImmutableList<KeyValuePair<string, string>>.Empty,
            Description = description ?? string.Empty,
            Images = images ?? ImmutableList<string>.Empty,
            Language = language
        };
    }

    private static HtmlDocument CreateHtmlDocument(string html)
    {
        var document = new HtmlDocument();
        document.LoadHtml(html);

        return document;
    }
}