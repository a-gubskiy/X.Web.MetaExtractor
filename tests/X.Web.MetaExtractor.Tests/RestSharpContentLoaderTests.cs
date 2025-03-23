using System;
using System.Net;
using System.Threading.Tasks;
using X.Web.MetaExtractor.ContentLoaders.RestSharp;
using Xunit;

namespace X.Web.MetaExtractor.Tests;

public class RestSharpContentLoaderTests
{
    private readonly IContentLoader _contentLoader;
    private readonly TimeSpan _timeout = TimeSpan.FromSeconds(10);

    public RestSharpContentLoaderTests()
    {
        _contentLoader = new RestSharpContentLoader();
    }

    [Theory]
    [InlineData("https://example.com", "<html")]
    [InlineData("https://en.wikipedia.org/wiki/Oceanic_whitetip_shark", "Oceanic whitetip shark")]
    public async Task Load_ValidUrl_ReturnsContentWithExpectedMarker(string url, string expectedContentMarker)
    {
        // Arrange
        var uri = new Uri(url);

        // Act
        var task = _contentLoader.Load(uri);
        var content = await task.TimeoutAfter(_timeout);

        // Assert
        Assert.NotNull(content);
        Assert.Contains(expectedContentMarker, content);
    }

    [Fact]
    public async Task Load_MultipleValidUrls_ReturnsContentForAll()
    {
        // Arrange
        var uri1 = new Uri("https://example.com");
        var uri2 = new Uri("https://example.org");

        // Act
        var task1 = _contentLoader.Load(uri1);
        var task2 = _contentLoader.Load(uri2);
        var results = await Task.WhenAll(task1, task2).TimeoutAfter(_timeout);

        // Assert
        Assert.All(results, content => Assert.False(string.IsNullOrEmpty(content)));
    }

    [Fact]
    public async Task Load_InvalidUrl_ThrowsException()
    {
        // Arrange
        var invalidUri = new Uri("https://invalid-domain-that-does-not-exist.com");

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() =>
            _contentLoader.Load(invalidUri).TimeoutAfter(_timeout));
    }
}

public static class TaskExtensions
{
    public static async Task<T> TimeoutAfter<T>(this Task<T> task, TimeSpan timeout)
    {
        var delayTask = Task.Delay(timeout);
        var completedTask = await Task.WhenAny(task, delayTask);

        if (completedTask == delayTask)
        {
            throw new TimeoutException($"Operation timed out after {timeout.TotalSeconds} seconds");
        }

        return await task;
    }
}