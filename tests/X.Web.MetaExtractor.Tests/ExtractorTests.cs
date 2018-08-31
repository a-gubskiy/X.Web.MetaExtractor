using System;
using System.Threading.Tasks;
using Xunit;

namespace X.Web.MetaExtractor.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task TestExtractMethod()
        {
            var extractor = new Extractor();

            var metaData1 = await extractor.ExtractAsync(new Uri("http://andrew.gubskiy.com/"));
            var metaData2 = await extractor.ExtractAsync(new Uri("http://torf.tv/"));
            var metaData3 = await extractor.ExtractAsync(new Uri("http://torf.tv/video/IraSkladPortrait"));
            var metaData4 = await extractor.ExtractAsync(new Uri("http://www.c-sharpcorner.com/news/stratis-bitcoin-full-node-for-net-core-in-c-sharp-goes-live"));
            var metaData5 = await extractor.ExtractAsync(new Uri("http://www.aaronstannard.com/the-coming-dotnet-reinassance/"));
            var metaData6 = await extractor.ExtractAsync(new Uri("https://bleacherreport.com/articles/2793475-nfl-preseason-week-4-roundup-bills-qb-battle-likely-down-to-allen-and-peterman"));
            
            Assert.NotNull(metaData1);
            Assert.NotNull(metaData2);
            Assert.NotNull(metaData3);
            Assert.NotNull(metaData4);
            Assert.NotNull(metaData5);
            Assert.NotEmpty(metaData6.Keywords);
        }
        
        [Fact]
        public async Task TestExtractLanguageData()
        {
            var extractor = new Extractor("", new PageContentLoader(), new LanguageDetector());
          
            var metaData = await extractor.ExtractAsync(new Uri("https://diepresse.com/home/panorama/wien/5386805/Polizist-attackiert_Parlament-verstaerkt-Bewachung"));
            
            Assert.Equal("de", metaData.Language);
        }
    }
}