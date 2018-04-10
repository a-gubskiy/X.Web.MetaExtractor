using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;

namespace X.Web.MetaExtractor
{
    public class Extractor
    {
        public string DefaultImage { get; set; }

        public async Task<Metadata> Extract(Uri uri)
        {
            var html = await LoadPageHtml(uri);

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

            if (!images.Any() && String.IsNullOrEmpty(image) && !String.IsNullOrEmpty(DefaultImage))
            {
                images = new List<string> {DefaultImage};
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

            return new Metadata
            {
                Title = title.Trim(),
                Description = description.Trim(),
                Content = content,
                Images = images,
                Url = uri.ToString()
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

        private static async Task<string> LoadPageHtml(Uri uri)
        {
            var handler = new HttpClientHandler {AllowAutoRedirect = true};

            var client = new HttpClient(handler);

            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "curl/7.54.0 (X.Web.MetaExtractor)");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "UTF-8");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "text/html; charset=UTF-8");

            var bytes = await client.GetByteArrayAsync(uri);

            return await ReadFromResponse(bytes);
        }

        private static async Task<string> ReadFromResponse(byte[] bytes)
        {
            try
            {
                return await ReadFromGzipStream(new MemoryStream(bytes));
            }
            catch
            {
                return await ReadFromStandardStream(new MemoryStream(bytes));
            }
        }

        private static async Task<string> ReadFromStandardStream(Stream stream)
        {
            using (var reader = new StreamReader(stream))
                return await reader.ReadToEndAsync();
        }

        private static async Task<string> ReadFromGzipStream(Stream stream)
        {
            using (var deflateStream = new GZipStream(stream, CompressionMode.Decompress))
            using (var reader = new StreamReader(deflateStream))
                return await reader.ReadToEndAsync();
        }

        private static string ReadOpenGraphProperty(HtmlDocument document, string name)
        {
            var node = document.DocumentNode.SelectSingleNode($"//meta[@property='{name}']");
            return HtmlDecode(node?.Attributes["content"]?.Value);
        }

        private static List<string> GetPageImages(HtmlDocument document)
            => document.DocumentNode.Descendants("img")
                .Select(e => e.GetAttributeValue("src", null))
                .Where(s => !String.IsNullOrEmpty(s))
                .ToList();

        private static string HtmlDecode(string text) =>
            string.IsNullOrWhiteSpace(text) ? string.Empty : WebUtility.HtmlDecode(text);
    }
}