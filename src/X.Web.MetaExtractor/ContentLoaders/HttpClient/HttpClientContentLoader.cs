using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace X.Web.MetaExtractor.ContentLoaders.HttpClient;

[PublicAPI]
public class HttpClientContentLoader : IContentLoader
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _httpClientName;

    public HttpClientContentLoader(IHttpClientFactory httpClientFactory)
        : this(httpClientFactory, "PageContentLoaderHttpClient")
    {
    }

    public HttpClientContentLoader(IHttpClientFactory httpClientFactory, string httpClientName)
    {
        _httpClientName = httpClientName;
        _httpClientFactory = httpClientFactory;
    }

    public HttpClientContentLoader()
        : this(new HttpClientFactory())
    {
    }

    public async Task<string> Load(Uri uri, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        var client = _httpClientFactory.CreateClient(_httpClientName);
        var response = await client.SendAsync(request, cancellationToken);
        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);

        return await ReadFromResponse(bytes);
    }

    protected static async Task<string> ReadFromResponse(byte[]? bytes)
    {
        if (bytes == null)
        {
            return string.Empty;
        }

        try
        {
            return await ReadFromGzipStream(new MemoryStream(bytes));
        }
        catch
        {
            return await ReadFromStandardStream(new MemoryStream(bytes));
        }
    }

    private static async Task<string> ReadFromStandardStream(Stream stream)
    {
        using (var reader = new StreamReader(stream))
        {
            return await reader.ReadToEndAsync();
        }
    }

    private static async Task<string> ReadFromGzipStream(Stream stream)
    {
        using (var deflateStream = new GZipStream(stream, CompressionMode.Decompress))
        using (var reader = new StreamReader(deflateStream))
        {
            return await reader.ReadToEndAsync();
        }
    }
}