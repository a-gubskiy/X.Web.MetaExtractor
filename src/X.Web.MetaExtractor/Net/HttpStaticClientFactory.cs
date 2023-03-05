using System;
using System.Net.Http;
using JetBrains.Annotations;

namespace X.Web.MetaExtractor.Net;

/// <summary>
/// ClientFactory which always return same client
/// </summary>
[PublicAPI]
public class HttpStaticClientFactory : IHttpClientFactory
{
    private static readonly HttpClient HttpClient;

    public HttpClient CreateClient(string name) => HttpClient;

    static HttpStaticClientFactory()
    {
        var handler = new HttpClientHandler {AllowAutoRedirect = true};

        var client = new HttpClient(handler) {Timeout = TimeSpan.FromSeconds(5)};

        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
        client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "curl/7.54.0 (X.Web.MetaExtractor)");
        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "UTF-8");
        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "text/html; charset=UTF-8");

        HttpClient = client;
    }
}