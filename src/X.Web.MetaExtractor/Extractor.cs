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

            html = System.Net.WebUtility.UrlDecode(html);

            var document = new HtmlDocument();
            document.LoadHtml(html);

            //Trye parse Open Graph properties
            var title = ReadOpenGraphProperty(document, "og:title");
            var image = ReadOpenGraphProperty(document, "og:image");
            var description = ReadOpenGraphProperty(document, "og:description");

            var content = CleanupContent(html);

            var images = GetPageImages(document);

            if (String.IsNullOrEmpty(title))
            {
                var node = document.DocumentNode.SelectSingleNode("//head/title");
                title = node != null ? node.InnerText : "";
            }

            if (!String.IsNullOrEmpty(image))
            {
                images = new List<string> { image };
            }

            if (!images.Any() && String.IsNullOrEmpty(image) && !String.IsNullOrEmpty(DefaultImage))
            {
                images = new List<string> { DefaultImage };
            }

            if (String.IsNullOrEmpty(description))
            {
                var node = document.DocumentNode.SelectSingleNode("//meta[@name='description']");
                description = node != null ? node.Attributes["content"].Value : "";
            }

            if (String.IsNullOrEmpty(description))
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(content);
                var text = doc.DocumentNode.InnerText;

                var length = text.Length >= 300 ? 300 : text.Length;
                description = text.Substring(0, length);
            }

            return new Metadata
            {
                Title = title.Trim(),
                Description = description.Trim(),
                Content = content,
                Image = images,
                Type = "webpage",
                Url = uri.ToString()
            };
        }

        private static string CleanupContent(string data)
        {
            if (string.IsNullOrEmpty(data)) return string.Empty;

            var document = new HtmlDocument();
            document.LoadHtml(data);

            document.DocumentNode.Descendants()
                .Where(n => n.Name == "script" || n.Name == "style" || n.InnerText.StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase))
                .ToList()
                .ForEach(n => n.Remove());

            var acceptableTags = new String[] { "strong", "em", "u", "img", "i" };

            var nodes = new Queue<HtmlNode>(document.DocumentNode.SelectNodes("./*|./text()"));

            while (nodes.Count > 0)
            {
                var node = nodes.Dequeue();
                var parentNode = node.ParentNode;

                if (!acceptableTags.Contains(node.Name) && node.Name != "#text")
                {
                    var childNodes = node.SelectNodes("./*|./text()");

                    if (childNodes != null)
                    {
                        foreach (var child in childNodes)
                        {
                            nodes.Enqueue(child);
                            parentNode.InsertBefore(child, node);
                        }
                    }

                    parentNode.RemoveChild(node);
                }
            }

            var content = document.DocumentNode.InnerHtml.Trim();
            content = Regex.Replace(content, @"[\r\n]{2,}", "<br />");
            return content;
        }

        private static async Task<string> LoadPageHtml(Uri uri)
        {
            var handler = new HttpClientHandler { AllowAutoRedirect = true };

            var client = new HttpClient(handler);

            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "UTF-8");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "text/html; charset=UTF-8");

            var responseStream = await client.GetStreamAsync(uri);
            var bytes = await client.GetByteArrayAsync(uri);

            var html = ReadFromResponse(bytes);
            return WebUtility.HtmlDecode(html);
        }

        private static string ReadFromResponse(byte[] bytes)
        {
            try
            {
                return ReadFromGzipStream(new MemoryStream(bytes));
            }
            catch
            {
                return ReadFromStandardStream(new MemoryStream(bytes));
            }
        }

        private static string ReadFromStandardStream(Stream stream)
        {
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        private static string ReadFromGzipStream(Stream stream)
        {
            using (var deflateStream = new GZipStream(stream, CompressionMode.Decompress))
            using (var reader = new StreamReader(deflateStream))
                return reader.ReadToEnd();
        }

        private static List<string> GetPageImages(HtmlDocument document)
        {
            return document.DocumentNode.Descendants("img")
                  .Select(e => e.GetAttributeValue("src", null))
                  .Where(s => !String.IsNullOrEmpty(s))
                  .ToList();
        }

        private static string ReadOpenGraphProperty(HtmlDocument document, string name)
        {
            var node = document.DocumentNode.SelectSingleNode($"//meta[@property='{name}']");

            if (node?.Attributes["content"] != null)
            {
                return node.Attributes["content"].Value;
            }

            return string.Empty;
        }
    }
}
