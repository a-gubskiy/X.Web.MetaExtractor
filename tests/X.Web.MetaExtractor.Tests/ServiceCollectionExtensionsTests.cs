using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using X.Web.MetaExtractor.ContentLoaders.HttpClient;
using X.Web.MetaExtractor.LanguageDetectors;
using Xunit;

namespace X.Web.MetaExtractor.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddMetaExtractor_RegistersDefaultServices()
    {
        using var provider = new ServiceCollection()
            .AddMetaExtractor()
            .BuildServiceProvider();

        Assert.IsType<Extractor>(provider.GetRequiredService<IExtractor>());
        Assert.IsType<HttpClientContentLoader>(provider.GetRequiredService<IContentLoader>());
        Assert.IsType<LanguageDetector>(provider.GetRequiredService<ILanguageDetector>());
    }

    [Fact]
    public void AddMetaExtractor_AppliesOptionsToHttpClient()
    {
        using var provider = new ServiceCollection()
            .AddMetaExtractor(options =>
            {
                options.MaxDescriptionLength = 42;
                options.ContentLoader.Timeout = TimeSpan.FromSeconds(17);
                options.ContentLoader.UserAgent = "TestAgent/1.0";
            })
            .BuildServiceProvider();

        var client = provider
            .GetRequiredService<IHttpClientFactory>()
            .CreateClient(HttpClientContentLoader.DefaultHttpClientName);

        Assert.Equal(TimeSpan.FromSeconds(17), client.Timeout);
        Assert.Equal("TestAgent/1.0", client.DefaultRequestHeaders.GetValues("User-Agent").Single());

        var extractor = Assert.IsType<Extractor>(provider.GetRequiredService<IExtractor>());

        Assert.Equal(42, extractor.MaxDescriptionLength);
    }

    [Fact]
    public async Task AddMetaExtractor_KeepsCustomContentLoader()
    {
        using var provider = new ServiceCollection()
            .AddSingleton<IContentLoader, StubContentLoader>()
            .AddMetaExtractor()
            .BuildServiceProvider();

        Assert.IsType<StubContentLoader>(provider.GetRequiredService<IContentLoader>());

        var page = await provider.GetRequiredService<IExtractor>().Extract(new Uri("https://example.com"));

        Assert.Equal("Stub", page.Title);
    }

    [Fact]
    public void HttpClientFactory_AppliesOptions()
    {
        var factory = new HttpClientFactory(new HttpClientContentLoaderOptions
        {
            Timeout = TimeSpan.FromSeconds(3),
            UserAgent = "Custom/2.0"
        });

        var client = factory.CreateClient("test");

        Assert.Same(client, factory.CreateClient("test"));
        Assert.Equal(TimeSpan.FromSeconds(3), client.Timeout);
        Assert.Equal("Custom/2.0", client.DefaultRequestHeaders.GetValues("User-Agent").Single());
    }

    private class StubContentLoader : IContentLoader
    {
        public Task<string> Load(Uri uri, CancellationToken cancellationToken) =>
            Task.FromResult("<html><head><title>Stub</title></head><body></body></html>");
    }
}
