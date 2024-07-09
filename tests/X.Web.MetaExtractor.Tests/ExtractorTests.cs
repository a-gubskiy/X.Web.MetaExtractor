using System;
using System.Linq;
using System.Threading.Tasks;
using X.Web.MetaExtractor.ContentLoaders.RestSharp;
using Xunit;

namespace X.Web.MetaExtractor.Tests;

public class ExtractorTests
{
    [Fact]
    public async Task TestExtractMethod()
    {
        var extractor = new Extractor();

        var metaData1 = await extractor.ExtractAsync(new Uri("http://andrew.gubskiy.com/"));
        var metaData2 = await extractor.ExtractAsync(new Uri("http://torf.tv/"));
        var metaData3 = await extractor.ExtractAsync(new Uri("http://torf.tv/video/IraSkladPortrait"));
        var metaData4 = await extractor.ExtractAsync(new Uri("http://www.c-sharpcorner.com/news/stratis-bitcoin-full-node-for-net-core-in-c-sharp-goes-live"));
        var metaData5 = await extractor.ExtractAsync(new Uri("https://devdigest.today/platform/"));

        Assert.NotNull(metaData1);
        Assert.NotNull(metaData2);
        Assert.NotNull(metaData3);
        Assert.NotNull(metaData4);
        Assert.NotNull(metaData5);
            
        Assert.NotEmpty(metaData3.Keywords);
    }
        
    [Fact]
    public async Task TestExtractLanguageData()
    {
        IPageContentLoader pageContentLoader = new RestSharpPageContentLoader();
        
        var languageDetector = new LanguageDetector();
        var extractor = new Extractor("", pageContentLoader, languageDetector);
          
        var metaData = await extractor.ExtractAsync(new Uri("https://diepresse.com/home/panorama/wien/5386805/Polizist-attackiert_Parlament-verstaerkt-Bewachung"));
            
        Assert.Equal("de", metaData.Language);
    }
        
    [Fact]
    public async Task TestExtractMetaTags()
    {
        IPageContentLoader pageContentLoader = new RestSharpPageContentLoader();
        
        var languageDetector = new LanguageDetector();
        var extractor = new Extractor("", pageContentLoader, languageDetector);
          
        var metaData1 = await extractor.ExtractAsync(new Uri("https://codeshare.co.uk/blog/how-to-scrape-meta-data-from-a-url-using-htmlagilitypack-in-c/"));
        var metaData2 = await extractor.ExtractAsync(new Uri("https://diepresse.com/home/techscience/5526578/Daten-sichern_WhatsApp-loescht-am-12-November-Chatverlaeufe"));
            
        var articleSection = metaData2.MetaTags.Where(o => o.Key == "og:description").Select(o => o.Value).FirstOrDefault();
            
        Assert.NotEmpty(metaData1.MetaTags);
        Assert.NotEmpty(articleSection);
    }
}