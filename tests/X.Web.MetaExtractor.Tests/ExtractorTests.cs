using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.Web.MetaExtractor.ContentLoaders.RestSharp;
using X.Web.MetaExtractor.LanguageDetectors;
using Xunit;

namespace X.Web.MetaExtractor.Tests;

public class ExtractorTests
{
    private readonly IContentLoader _contentLoader;
    private readonly ILanguageDetector _languageDetector;

    private static readonly Dictionary<string, Uri> TestUrls = new()
    {
        ["Personal"] = new Uri("http://andrew.gubskiy.com/"),
        ["TorfTV"] = new Uri("http://torf.tv/"),
        ["TorfTVVideo"] = new Uri("http://torf.tv/video/IraSkladPortrait"),
        ["CSharpCorner"] =
            new Uri("http://www.c-sharpcorner.com/news/stratis-bitcoin-full-node-for-net-core-in-c-sharp-goes-live"),
        ["DevDigest"] = new Uri("https://devdigest.today/platform/"),
        ["GermanArticle"] =
            new Uri(
                "https://diepresse.com/home/panorama/wien/5386805/Polizist-attackiert_Parlament-verstaerkt-Bewachung"),
        ["CodeShare"] =
            new Uri("https://codeshare.co.uk/blog/how-to-scrape-meta-data-from-a-url-using-htmlagilitypack-in-c/"),
        ["WhatsApp"] =
            new Uri(
                "https://diepresse.com/home/techscience/5526578/Daten-sichern_WhatsApp-loescht-am-12-November-Chatverlaeufe")
    };

    public ExtractorTests()
    {
        _contentLoader = new RestSharpContentLoader();
        _languageDetector = new LanguageDetector();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Extract_ValidUrls_ReturnsValidMetadata()
    {
        // Arrange
        IExtractor extractor = new Extractor();

        // Act
        var metaData1 = await extractor.Extract(TestUrls["Personal"]);
        var metaData2 = await extractor.Extract(TestUrls["TorfTV"]);
        var metaData3 = await extractor.Extract(TestUrls["TorfTVVideo"]);
        var metaData4 = await extractor.Extract(TestUrls["CSharpCorner"]);
        var metaData5 = await extractor.Extract(TestUrls["DevDigest"]);

        // Assert
        Assert.NotNull(metaData1);
        Assert.NotNull(metaData2);
        Assert.NotNull(metaData3);
        Assert.NotNull(metaData4);
        Assert.NotNull(metaData5);

        Assert.NotEmpty(metaData3.Keywords);
        Assert.NotNull(metaData1.Title);
        Assert.NotNull(metaData2.Title);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Extract_GermanWebsite_DetectsLanguageCorrectly()
    {
        // Arrange
        IExtractor extractor = new Extractor("", _contentLoader, _languageDetector);

        // Act
        var metaData = await extractor.Extract(TestUrls["GermanArticle"]);

        // Assert
        Assert.Equal("de", metaData.Language);
        Assert.NotNull(metaData.Title);
        Assert.NotEmpty(metaData.Title);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Extract_WebsitesWithMetadata_ReturnsCorrectMetaTags()
    {
        // Arrange
        IExtractor extractor = new Extractor("", _contentLoader, _languageDetector);

        // Act
        var webPage1 = await extractor.Extract(TestUrls["CodeShare"]);
        var webPage2 = await extractor.Extract(TestUrls["WhatsApp"]);

        // Assert
        Assert.NotEmpty(webPage1.Metadata);

        var articleDescription = webPage2.Metadata
            .FirstOrDefault(o => o.Key == "og:description");

        Assert.NotNull(articleDescription.Value);
        Assert.NotEmpty(articleDescription.Key);
    }

    [Fact]
    [Trait("Category", "Error")]
    public async Task Extract_InvalidUrl_HandlesErrorGracefully()
    {
        // Arrange
        IExtractor extractor = new Extractor("", _contentLoader, _languageDetector);

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await extractor.Extract(new Uri("https://invalid-domain-that-does-not-exist.com")));
    }

    [Theory]
    [InlineData("Personal", true)]
    [InlineData("TorfTV", true)]
    [InlineData("DevDigest", true)]
    [Trait("Category", "Integration")]
    public async Task Extract_VariousUrls_ReturnsExpectedResults(string urlKey, bool shouldHaveTitle)
    {
        // Arrange
        IExtractor extractor = new Extractor("", _contentLoader, _languageDetector);

        // Act
        var result = await extractor.Extract(TestUrls[urlKey]);

        // Assert
        Assert.NotNull(result);
        if (shouldHaveTitle)
        {
            Assert.NotEmpty(result.Title);
        }
    }
}