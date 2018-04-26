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
        }
    }
}