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
    private readonly IContentLoader _contentLoader;
    private readonly ILanguageDetector _languageDetector;

    private readonly TitleExtractor _titleExtractor;
    private readonly KeywordsExtractor _keywordsExtractor;
    private readonly MetaTagsExtractor _metaTagsExtractor;
    private readonly DescriptionExtractor _descriptionExtractor;
    private readonly ImageExtractor _imageExtractor;

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
        _defaultImage = defaultImage;
        _languageDetector = languageDetector;
        _contentLoader = contentLoader;
        _titleExtractor = new TitleExtractor();
        _keywordsExtractor = new KeywordsExtractor();
        _metaTagsExtractor = new MetaTagsExtractor();
        _descriptionExtractor = new DescriptionExtractor();
        _imageExtractor = new ImageExtractor(_defaultImage);
    }

    /// <inheritdoc />
    public async Task<Metadata> Extract(Uri uri, CancellationToken cancellationToken)
    {
        var html = await _contentLoader.LoadPageContent(uri, cancellationToken);

        var document = CreateHtmlDocument(html);

        var title = _titleExtractor.ExtractTitle(document);
        var keywords = _keywordsExtractor.ExtractKeywords(document);
        var meta = _metaTagsExtractor.ExtractMeta(document);
        var description = _descriptionExtractor.ExtractDescription(document);
        var images = _imageExtractor.ExtractImages(document);
        var language = _languageDetector.GetHtmlPageLanguage(html);

        return new Metadata
        {
            Raw = html,
            Url = uri,

            Title = title,
            Keywords = keywords,
            MetaTags = meta,
            Description = description,
            Images = images,
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