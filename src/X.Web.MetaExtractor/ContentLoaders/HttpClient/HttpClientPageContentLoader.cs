using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace X.Web.MetaExtractor.ContentLoaders.HttpClient;

[PublicAPI]
public class HttpClientPageContentLoader : IPageContentLoader
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _httpClientName;

    public HttpClientPageContentLoader(IHttpClientFactory httpClientFactory)
        : this(httpClientFactory, "PageContentLoaderHttpClient")
    {
    }

    public HttpClientPageContentLoader(IHttpClientFactory httpClientFactory, string httpClientName)
    {
        _httpClientName = httpClientName;
        _httpClientFactory = httpClientFactory;
    }

    public HttpClientPageContentLoader()
        : this(new HttpClientFactory())
    {
    }

    public async Task<string> LoadPageContent(Uri uri, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        var client = _httpClientFactory.CreateClient(_httpClientName);
        var response = await client.SendAsync(request, cancellationToken);
        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);

        return await ReadFromResponseAsync(bytes);
    }

    protected static async Task<string> ReadFromResponseAsync(byte[]? bytes)
    {
        if (bytes == null)
        {
            return string.Empty;
        }

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
        {
            return await reader.ReadToEndAsync();
        }
    }

    private static async Task<string> ReadFromGzipStreamAsync(Stream stream)
    {
        using (var deflateStream = new GZipStream(stream, CompressionMode.Decompress))
        using (var reader = new StreamReader(deflateStream))
        {
            return await reader.ReadToEndAsync();
        }
    }
}