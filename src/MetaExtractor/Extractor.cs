using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace MetaExtractor
{
    public class Extractor
    {
        public string DefaultImage { get; set; }

        public async Task<Metadata> Extract(Uri uri)
        {
            var client = new HttpClient();

            var bytes = await client.GetByteArrayAsync(uri);
            var html = Encoding.UTF8.GetString(bytes);

            var document = new HtmlDocument();
            document.LoadHtml(html);

            var title = ReadOpenGraphProperty(document, "og:title");
            var image = ReadOpenGraphProperty(document, "og:image");
            var description = ReadOpenGraphProperty(document, "og:description");

            if (String.IsNullOrEmpty(title))
            {
                var node = document.DocumentNode.SelectSingleNode("//head/title");
                title = node != null ? node.InnerText : "";
            }

            if (String.IsNullOrEmpty(image))
            {
                var images = document.DocumentNode.Descendants("img")
                    .Select(e => e.GetAttributeValue("src", null))
                    .Where(s => !String.IsNullOrEmpty(s))
                    .ToList();

                image = images.FirstOrDefault();
            }

            if (String.IsNullOrEmpty(image) && !String.IsNullOrEmpty(DefaultImage))
            {
                image = DefaultImage;
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
                Title = title,
                Description = description,
                Image = image,
                Type = "webpage",
                Url = uri.ToString()
            };
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