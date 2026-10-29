using System.Collections.Concurrent;
using System.Net.Http;
using JetBrains.Annotations;

namespace X.Web.MetaExtractor.ContentLoaders.HttpClient;

/// <summary>
/// Minimal <see cref="IHttpClientFactory"/> for use without dependency injection.
/// Creates one <see cref="System.Net.Http.HttpClient"/> per name and reuses it,
/// so keep a single factory instance for the lifetime of the application.
/// </summary>
[PublicAPI]
public class HttpClientFactory : IHttpClientFactory
{
    private readonly ConcurrentDictionary<string, System.Net.Http.HttpClient> _clients = new();

    public HttpClientFactory()
        : this(new HttpClientContentLoaderOptions())
    {
    }

    public HttpClientFactory(HttpClientContentLoaderOptions options)
    {
        Options = options;
    }

    protected HttpClientContentLoaderOptions Options { get; }

    public System.Net.Http.HttpClient CreateClient(string name) => _clients.GetOrAdd(name, Create);

    protected virtual System.Net.Http.HttpClient Create(string key)
    {
        var handler = new HttpClientHandler { AllowAutoRedirect = true };

        var client = new System.Net.Http.HttpClient(handler);

        Options.Apply(client);

        return client;
    }
}
