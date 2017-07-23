using System;
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


            if (String.IsNullOrEmpty(image) && !String.IsNullOrEmpty(DefaultImage))
            {
                image = DefaultImage;
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


            if (node != null && node.Attributes["content"] != null)
            {
                return node.Attributes["content"].Value;
            }

            return string.Empty;
        }
    }
}