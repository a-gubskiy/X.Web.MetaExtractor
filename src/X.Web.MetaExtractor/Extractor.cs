using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using JetBrains.Annotations;
using X.Web.MetaExtractor.ContentLoaders.HttpClient;

namespace X.Web.MetaExtractor;

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

    public async Task<Metadata> ExtractAsync(Uri uri)
    {
        var html = await _pageContentLoader.LoadPageContentAsync(uri);

        return Extract(uri, html);
    }

    /// <summary>
    /// Extract metadata from HTML.
    /// Store uri as Url field
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="html"></param>
    /// <returns></returns>
    [PublicAPI]
    public Metadata Extract(Uri uri, string html)
    {
        var document = CreateHtmlDocument(html);

        //Try parse Open Graph properties
        var title = ReadOpenGraphProperty(document, "og:title");
        var image = ReadOpenGraphProperty(document, "og:image");
        var description = ReadOpenGraphProperty(document, "og:description");
        var keywords = ExtractKeywords(document);
        var content = CleanupContent(html);
        var images = GetPageImages(document);
        var metatags = GetMetaTags(document);
        var language = _languageDetector.GetHtmlPageLanguage(html);

        if (string.IsNullOrWhiteSpace(title))
        {
            var node = document.DocumentNode.SelectSingleNode("//head/title");
            
            title = node != null ? HtmlDecode(node.InnerText) : "";
        }

        if (!string.IsNullOrWhiteSpace(image))
        {
            //When image defined via Open Graph
            images = new List<string> {image};
        }

        if (!images.Any() && string.IsNullOrWhiteSpace(image) && !string.IsNullOrWhiteSpace(_defaultImage))
        {
            images = new List<string> {_defaultImage};
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            description = ExtractDescription(document);
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            var doc = CreateHtmlDocument(content);
            var text = doc.DocumentNode.InnerText;
            var length = text.Length >= MaxDescriptionLength ? MaxDescriptionLength : text.Length;
                
            description = text[..length];
        }

        return new Metadata
        {
            Title = title,
            Keywords = keywords,
            MetaTags = metatags,
            Description = description,
            Content = content,
            Raw = html,
            Images = images,
            Url = uri.ToString(),
            Language = language
        };
    }
        
    private static IReadOnlyCollection<KeyValuePair<string, string>> GetMetaTags(HtmlDocument document)
    {
        var result = new List<KeyValuePair<string, string>>();

        var list = document?.DocumentNode?.SelectNodes("//meta");

        if (list == null || !list.Any())
        {
            return new List<KeyValuePair<string, string>>();
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

    private static IReadOnlyCollection<string> ExtractKeywords(HtmlDocument document)
    {
        var node = document.DocumentNode.SelectSingleNode("//meta[@name='keywords']");
        var value = node != null ? HtmlDecode(node?.Attributes["content"]?.Value) : "";

        if (string.IsNullOrWhiteSpace(value))
        {
            return ImmutableArray<string>.Empty;
        }

        return value.Split(',').Select(o => o?.Trim()).Where(o => !string.IsNullOrWhiteSpace(o)).ToImmutableList();
    }

    private static string ExtractDescription(HtmlDocument document)
    {
        var node = document.DocumentNode.SelectSingleNode("//meta[@name='description']");
        return node != null ? HtmlDecode(node?.Attributes["content"]?.Value) : string.Empty;
    }

    private static HtmlDocument CreateHtmlDocument(string html)
    {
        var document = new HtmlDocument();
        document.LoadHtml(html ?? string.Empty);
        
        return document;
    }

    private static string CleanupContent(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            return string.Empty;
        }

        var document = new HtmlDocument();
        document.LoadHtml(data);

        document.DocumentNode
            .Descendants()
            .Where(n => n.Name == "script" ||
                        n.Name == "style" ||
                        n.InnerText.StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase))
            .ToList()
            .ForEach(n => n.Remove());

        var acceptableTags = new[] {"strong", "em", "u", "img", "i"};

        var nodes = new Queue<HtmlNode>(document.DocumentNode.SelectNodes("./*|./text()"));

        while (nodes.Count > 0)
        {
            var node = nodes.Dequeue();

            if (!acceptableTags.Contains(node.Name) && node.Name != "#text")
            {
                var childNodes = node.SelectNodes("./*|./text()");

                if (childNodes != null)
                {
                    foreach (var child in childNodes)
                    {
                        nodes.Enqueue(child);
                        node.ParentNode.InsertBefore(child, node);
                    }
                }

                node.ParentNode.RemoveChild(node);
            }
        }

        var content = document.DocumentNode.InnerHtml.Trim();

        return Regex.Replace(content, @"[\r\n]{2,}", "<br />");
    }

    private static string ReadOpenGraphProperty(HtmlDocument document, string name)
    {
        var node = document.DocumentNode.SelectSingleNode($"//meta[@property='{name}']");
        return HtmlDecode(node?.Attributes["content"]?.Value)?.Trim() ?? string.Empty;
    }

    private static IReadOnlyCollection<string> GetPageImages(HtmlDocument document) =>
        document.DocumentNode.Descendants("img")
            .Select(e => e.GetAttributeValue("src", null))
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToImmutableList();

    private static string HtmlDecode(string text) =>
        string.IsNullOrWhiteSpace(text) ? string.Empty : WebUtility.HtmlDecode(text);
}