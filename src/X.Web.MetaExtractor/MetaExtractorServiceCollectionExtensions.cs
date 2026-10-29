using System;
using System.Net.Http;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using X.Web.MetaExtractor;
using X.Web.MetaExtractor.ContentLoaders.HttpClient;
using X.Web.MetaExtractor.LanguageDetectors;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Registers X.Web.MetaExtractor services in an <see cref="IServiceCollection"/>.
/// </summary>
[PublicAPI]
public static class MetaExtractorServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IExtractor"/> with <see cref="HttpClientContentLoader"/> backed by a named
    /// client from <see cref="IHttpClientFactory"/>, and <see cref="LanguageDetector"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="IContentLoader"/> and <see cref="ILanguageDetector"/> are registered with TryAdd,
    /// so implementations registered before this call are kept.
    /// </remarks>
    public static IServiceCollection AddMetaExtractor(
        this IServiceCollection services,
        Action<MetaExtractorOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = services.AddOptions<MetaExtractorOptions>();

        if (configure != null)
        {
            options.Configure(configure);
        }

        services.AddHttpClient(HttpClientContentLoader.DefaultHttpClientName, (provider, client) =>
            provider.GetRequiredService<IOptions<MetaExtractorOptions>>().Value.ContentLoader.Apply(client));

        services.TryAddSingleton<IContentLoader>(provider =>
            new HttpClientContentLoader(provider.GetRequiredService<IHttpClientFactory>()));

        services.TryAddSingleton<ILanguageDetector, LanguageDetector>();

        services.TryAddSingleton<IExtractor>(provider =>
        {
            var value = provider.GetRequiredService<IOptions<MetaExtractorOptions>>().Value;

            return new Extractor(
                value.DefaultImage,
                provider.GetRequiredService<IContentLoader>(),
                provider.GetRequiredService<ILanguageDetector>())
            {
                MaxDescriptionLength = value.MaxDescriptionLength
            };
        });

        return services;
    }
}
