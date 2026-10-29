using JetBrains.Annotations;
using X.Web.MetaExtractor.ContentLoaders.HttpClient;

namespace X.Web.MetaExtractor;

/// <summary>
/// Settings used by <c>AddMetaExtractor</c> to configure <see cref="Extractor"/> and its HTTP client.
/// </summary>
[PublicAPI]
public class MetaExtractorOptions
{
    /// <summary>
    /// Image URL returned when the page has no images.
    /// </summary>
    public string DefaultImage { get; set; } = string.Empty;

    /// <summary>
    /// Maximum length of the extracted description.
    /// </summary>
    public int MaxDescriptionLength { get; set; } = 300;

    /// <summary>
    /// Settings of the HTTP client used to load pages.
    /// </summary>
    public HttpClientContentLoaderOptions ContentLoader { get; set; } = new();
}
