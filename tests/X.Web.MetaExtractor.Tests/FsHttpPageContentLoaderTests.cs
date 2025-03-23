using System;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using X.Web.MetaExtractor.ContentLoaders.FsHttp;
using Xunit;

namespace X.Web.MetaExtractor.Tests;

public class FsHttpPageContentLoaderTests : IDisposable
{
    private readonly HttpClient _mockHttpClient;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly IContentLoader _loader;

    public FsHttpPageContentLoaderTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _mockHttpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _loader = new FsHttpPageContentLoader();
    }

    public void Dispose()
    {
        _mockHttpClient.Dispose();
        GC.SuppressFinalize(this);
    }
    
    [Fact]
    public async Task Load_MultipleUrls_ReturnsAllContents()
    {
        // Arrange
        var url = new Uri("https://example.com");
       
        // Act
        var value = await _loader.Load(url);

        Assert.Contains("<title>Example Domain</title>", value);
    }
    
}