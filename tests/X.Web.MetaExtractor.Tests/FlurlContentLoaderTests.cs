using System;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Testing;
using X.Web.MetaExtractor.ContentLoaders.Flurl;
using Xunit;

namespace X.Web.MetaExtractor.Tests;

public class FlurlContentLoaderTests : IDisposable
{
    private readonly HttpTest _httpTest;
    private readonly IContentLoader _contentLoader;

    public FlurlContentLoaderTests()
    {
        _httpTest = new HttpTest();
        _contentLoader = new FlurlContentLoader();
    }

    public void Dispose()
    {
        _httpTest.Dispose();
    }
    
    [Fact]
    public async Task Load_HttpErrorResponse_ThrowsException()
    {
        // Arrange
        var url = new Uri("https://example.com/notfound");
        _httpTest.RespondWith(status: 404);

        // Act & Assert
        await Assert.ThrowsAsync<FlurlHttpException>(() =>
            _contentLoader.Load(url));
    }

    [Theory]
    [InlineData("https://example.com")]
    [InlineData("https://example.org")]
    [InlineData("https://example.net")]
    public async Task Load_MultipleUrls_AllSucceed(string urlString)
    {
        // Arrange
        var url = new Uri(urlString);
        _httpTest.RespondWith($"Content for {urlString}", 200);

        // Act
        var result = await _contentLoader.Load(url);

        // Assert
        Assert.Contains(urlString, result);
    }

    [Fact]
    public async Task Load_ParallelRequests_AllSucceed()
    {
        // Arrange
        var urls = new[]
        {
            new Uri("https://site1.com"),
            new Uri("https://site2.com"),
            new Uri("https://site3.com")
        };

        _httpTest.RespondWith("Content 1", 200)
            .RespondWith("Content 2", 200)
            .RespondWith("Content 3", 200);

        // Act
        var tasks = urls.Select(url => _contentLoader.Load(url));
        var results = await Task.WhenAll(tasks);

        // Assert
        Assert.Equal(urls.Length, results.Length);
        Assert.All(results, Assert.NotNull);
        Assert.Contains("Content 1", results);
        Assert.Contains("Content 2", results);
        Assert.Contains("Content 3", results);
    }

    [Fact]
    public async Task Load_WithTimeout_ThrowsTimeoutException()
    {
        // Arrange
        var url = new Uri("https://slow-site.com");
        _httpTest.SimulateTimeout();

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() =>
            _contentLoader.Load(url));
    }
}