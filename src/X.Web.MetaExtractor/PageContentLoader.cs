using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace X.Web.MetaExtractor
{
    public class PageContentLoader : IPageContentLoader
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PageContentLoader(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        
        public PageContentLoader()
            : this(new HttpClientFactory())
        {
        }

        public async Task<string> LoadPageContentAsync(Uri uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(request);
            var bytes = await response.Content.ReadAsByteArrayAsync();

            return await ReadFromResponseAsync(bytes);
        }

        public string LoadPageContent(Uri uri) =>
            LoadPageContentAsync(uri).ConfigureAwait(false).GetAwaiter().GetResult();

        private static async Task<string> ReadFromResponseAsync(byte[] bytes)
        {
            try
            {
                return await ReadFromGzipStreamAsync(new MemoryStream(bytes));
            }
            catch
            {
                return await ReadFromStandardStreamAsync(new MemoryStream(bytes));
            }
        }

        private static async Task<string> ReadFromStandardStreamAsync(Stream stream)
        {
            using (var reader = new StreamReader(stream))
                return await reader.ReadToEndAsync();
        }

        private static async Task<string> ReadFromGzipStreamAsync(Stream stream)
        {
            using (var deflateStream = new GZipStream(stream, CompressionMode.Decompress))
            using (var reader = new StreamReader(deflateStream))
                return await reader.ReadToEndAsync();
        }
    }
}