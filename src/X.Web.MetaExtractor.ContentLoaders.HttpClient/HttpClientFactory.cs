using System;
using System.Collections.Concurrent;
using System.Net.Http;
using JetBrains.Annotations;

namespace X.Web.MetaExtractor.ContentLoaders.HttpClient;

[PublicAPI]
public class HttpClientFactory : IHttpClientFactory
{
    private static readonly ConcurrentDictionary<string, System.Net.Http.HttpClient> Clients = new();

    public System.Net.Http.HttpClient CreateClient(string name) => Clients.GetOrAdd(name, Create);

    private static System.Net.Http.HttpClient Create(string key)
    {
        var handler = new HttpClientHandler {AllowAutoRedirect = true};

        var client = new System.Net.Http.HttpClient(handler) {Timeout = TimeSpan.FromSeconds(5)};

        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
        client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "curl/7.54.0 (X.Web.MetaExtractor)");
        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "UTF-8");
        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "text/html; charset=UTF-8");
        
        return client;
    }
}