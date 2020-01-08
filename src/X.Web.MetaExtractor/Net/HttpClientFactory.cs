using System;
using System.Collections.Concurrent;
using System.Net.Http;

namespace X.Web.MetaExtractor.Net
{
    public class HttpClientFactory : IHttpClientFactory
    {
        private static readonly ConcurrentDictionary<string, HttpClient> Clients = new ConcurrentDictionary<string, HttpClient>();

        public HttpClient CreateClient(string name) => Clients.GetOrAdd(name, (key) => CreateClient());

        private static HttpClient CreateClient()
        {
            var handler = new HttpClientHandler {AllowAutoRedirect = true};

            var client = new HttpClient(handler) {Timeout = TimeSpan.FromSeconds(5)};

            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "curl/7.54.0 (X.Web.MetaExtractor)");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "UTF-8");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "text/html; charset=UTF-8");

            return client;
        }
    }
}