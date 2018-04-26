using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace X.Web.MetaExtractor
{
    public class PageContentLoader : IPageContentLoader
    {
        private readonly HttpClient _httpClient;
        private readonly TimeSpan _timeout;

        public PageContentLoader()
            : this(TimeSpan.FromSeconds(10))
        {
        }
        
        public PageContentLoader(TimeSpan timeout, bool useSingleHttpClient = false)
        {
            _timeout = timeout;
            
            if (useSingleHttpClient)
                _httpClient = CreateHttpClient(_timeout);
        }
        
        
        public async Task<string> LoadPageContentAsync(Uri uri)
        {
            var client = _httpClient ?? CreateHttpClient(_timeout);

            var bytes = await client.GetByteArrayAsync(uri);

            return await ReadFromResponseAsync(bytes);
        }

        public string LoadPageContent(Uri uri)
        {
            var queue = new ConcurrentQueue<string>();

            var client = _httpClient ?? CreateHttpClient(_timeout);

            Task.Run(() =>
            {
                return client.GetByteArrayAsync(uri).ContinueWith(response =>
                {
                    var bytes = response.GetAwaiter().GetResult();

                    var html = ReadFromResponse(bytes);
                    queue.Enqueue(html);
                });
            });

            return queue.SingleOrDefault();
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
        
        private static HttpClient CreateHttpClient(TimeSpan timeout)
        {
            var handler = new HttpClientHandler {AllowAutoRedirect = true};

            var client = new HttpClient(handler) {Timeout = timeout};

            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "curl/7.54.0 (X.Web.MetaExtractor)");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "UTF-8");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "text/html; charset=UTF-8");

            return client;
        }
        
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