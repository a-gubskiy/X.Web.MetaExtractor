using System;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace X.Web.MetaExtractor.ContentLoaders.FsHttp.Tests;

public class FsHttpPageContentLoaderTests : IDisposable
{
    private readonly System.Net.Http.HttpClient _mockHttpClient;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly IContentLoader _loader;

    public FsHttpPageContentLoaderTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _mockHttpClient = new System.Net.Http.HttpClient(_mockHttpMessageHandler.Object);
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