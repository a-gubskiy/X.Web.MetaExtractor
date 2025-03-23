using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using X.Web.MetaExtractor.ContentLoaders.HttpClient;
using Xunit;

namespace X.Web.MetaExtractor.Tests;

public class HttpClientContentLoaderTests
{
    private const string SampleHtml = "<html><head><title>Test Page</title></head><body>Content</body></html>";

    [Fact]
    public async Task Load_ValidUrl_ReturnsContent()
    {
        // Arrange
        var mockHttpFactory = new Mock<IHttpClientFactory>();
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(SampleHtml)
            });

        var httpClient = new System.Net.Http.HttpClient(mockHandler.Object);
        
        mockHttpFactory
            .Setup(o => o.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);
        
        IContentLoader contentLoader = new HttpClientContentLoader(mockHttpFactory.Object);

        // Act
        var result = await contentLoader.Load(new Uri("https://example.com"));

        // Assert
        Assert.NotNull(result);
        Assert.Contains("<title>Test Page</title>", result);
    }

    [Fact]
    public async Task Load_MultipleRequests_AllSucceed()
    {
        // Arrange
        var mockHttpFactory = new Mock<IHttpClientFactory>();
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(SampleHtml)
            });

        var httpClient = new System.Net.Http.HttpClient(mockHandler.Object);

        mockHttpFactory
            .Setup(o => o.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);
        
        IContentLoader contentLoader = new HttpClientContentLoader(mockHttpFactory.Object);

        // Act
        var tasks = new[]
        {
            contentLoader.Load(new Uri("https://example1.com")),
            contentLoader.Load(new Uri("https://example2.com")),
            contentLoader.Load(new Uri("https://example3.com"))
        };

        var results = await Task.WhenAll(tasks);

        // Assert
        Assert.All(results, content => Assert.Contains("<title>Test Page</title>", content));
    }

    [Fact]
    public async Task Load_NotFoundStatusCode_ReturnsNull()
    {
        // Arrange
        var mockHttpFactory = new Mock<IHttpClientFactory>();
        var mockHandler = new Mock<HttpMessageHandler>();
        
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });

        var httpClient = new System.Net.Http.HttpClient(mockHandler.Object);
        
        mockHttpFactory
            .Setup(o => o.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);
        
        IContentLoader contentLoader = new HttpClientContentLoader(mockHttpFactory.Object);

        // Act
        var result = await contentLoader.Load(new Uri("https://example.com/notfound"));

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Load_RequestTimeout_ThrowsException()
    {
        // Arrange
        var mockHttpFactory = new Mock<IHttpClientFactory>();
        var mockHandler = new Mock<HttpMessageHandler>();
        
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new TaskCanceledException());

        var httpClient = new System.Net.Http.HttpClient(mockHandler.Object);
        
        mockHttpFactory
            .Setup(o => o.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);
        
        IContentLoader contentLoader = new HttpClientContentLoader(mockHttpFactory.Object);

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(() =>
            contentLoader.Load(new Uri("https://example.com")));
    }
}