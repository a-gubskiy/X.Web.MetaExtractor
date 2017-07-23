using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace X.Web.MetaExtractor
{
    public class Extractor
    {
        public string DefaultImage { get; set; }

        public async Task<Metadata> Extract(Uri uri)
        {
            var html = await LoadPageHtml(uri);

            var document = new HtmlDocument();
            document.LoadHtml(html);

            //Trye parse Open Graph properties
            var title = ReadOpenGraphProperty(document, "og:title");
            var image = ReadOpenGraphProperty(document, "og:image");
            var description = ReadOpenGraphProperty(document, "og:description");

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
                var node = document.DocumentNode.SelectSingleNode("//body");
                var maxLength = node.InnerText.Length;
                var length = maxLength >= 300 ? 300 : maxLength;

                description = node.InnerText.Substring(0, length);
            }

            return new Metadata
            {
                Title = title.Trim(),
                Description = description.Trim(),
                Image = images,
                Type = "webpage",
                Url = uri.ToString()
            };
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

            var bytes = await client.GetByteArrayAsync(uri);
            var html = WebUtility.HtmlDecode(Encoding.UTF8.GetString(bytes));

            return html;
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