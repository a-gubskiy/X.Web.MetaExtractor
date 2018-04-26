using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace X.Web.MetaExtractor
{
    public class Extractor : IExtractor
    {
        private readonly string _defaultImage;
        private readonly IPageContentLoader _pageContentLoader;
        private readonly ILanguageDetector _languageDetector;
        
        public Extractor()
            : this("", TimeSpan.FromSeconds(10), false)
        {
        }

        public Extractor(string defaultImage, TimeSpan timeout, bool useSingleHttpClient)
            : this(defaultImage, new PageContentLoader(timeout, useSingleHttpClient), new FakeLanguageDetector())
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
        
        public Metadata Extract(Uri uri)
        {
            var html = _pageContentLoader.LoadPageContent(uri);

            return Extract(uri, html);
        }

        private Metadata Extract(Uri uri, string html)
        {
            var document = CreateHtmlDocument(html);

            //Trye parse Open Graph properties
            var title = ReadOpenGraphProperty(document, "og:title");
            var image = ReadOpenGraphProperty(document, "og:image");
            var description = ReadOpenGraphProperty(document, "og:description");

            var content = CleanupContent(html);

            var images = GetPageImages(document);

            if (String.IsNullOrEmpty(title))
            {
                var node = document.DocumentNode.SelectSingleNode("//head/title");
                title = node != null ? HtmlDecode(node.InnerText) : "";
            }

            if (!String.IsNullOrEmpty(image))
            {
                //When image defined via Open Graph
                images = new List<string> {image};
            }

            if (!images.Any() && String.IsNullOrEmpty(image) && !String.IsNullOrEmpty(_defaultImage))
            {
                images = new List<string> {_defaultImage};
            }

            if (String.IsNullOrEmpty(description))
            {
                var node = document.DocumentNode.SelectSingleNode("//meta[@name='description']");
                description = node != null ? HtmlDecode(node?.Attributes["content"]?.Value) : "";
            }

            if (String.IsNullOrEmpty(description))
            {
                var doc = CreateHtmlDocument(content);

                var text = doc.DocumentNode.InnerText;

                var length = text.Length >= 300 ? 300 : text.Length;
                description = text.Substring(0, length);
            }

            var language = _languageDetector.GetHtmlPageLanguage(html);
            
            return new Metadata
            {
                Title = title.Trim(),
                Description = description.Trim(),
                Content = content,
                Images = images,
                Url = uri.ToString(),
                Language = language
            };
        }

        private static HtmlDocument CreateHtmlDocument(string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            return document;
        }

        private static string CleanupContent(string data)
        {
            if (string.IsNullOrEmpty(data))
                return string.Empty;

            var document = new HtmlDocument();
            document.LoadHtml(data);

            document.DocumentNode.Descendants()
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
            return HtmlDecode(node?.Attributes["content"]?.Value);
        }

        private static List<string> GetPageImages(HtmlDocument document)
            => document.DocumentNode.Descendants("img")
                .Select(e => e.GetAttributeValue("src", null))
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();

        private static string HtmlDecode(string text) =>
            string.IsNullOrWhiteSpace(text) ? string.Empty : WebUtility.HtmlDecode(text);
    }
}