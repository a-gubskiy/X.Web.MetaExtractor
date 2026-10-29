using System;
using JetBrains.Annotations;

namespace X.Web.MetaExtractor.ContentLoaders.HttpClient;

/// <summary>
/// Settings applied to the <see cref="System.Net.Http.HttpClient"/> used by <see cref="HttpClientContentLoader"/>.
/// </summary>
[PublicAPI]
public class HttpClientContentLoaderOptions
{
    /// <summary>
    /// Request timeout. Defaults to 5 seconds.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Value of the User-Agent header sent with every request.
    /// </summary>
    public string UserAgent { get; set; } = "curl/7.54.0 (X.Web.MetaExtractor)";

    internal void Apply(System.Net.Http.HttpClient client)
    {
        client.Timeout = Timeout;

        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
        client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", UserAgent);
        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "UTF-8");
        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "text/html; charset=UTF-8");
    }
}
