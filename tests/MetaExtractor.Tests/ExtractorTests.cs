using System;
using System.Threading.Tasks;
using Xunit;

namespace MetaExtractor.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task TestExtractMethod()
        {
            var url = "https://andrew.gubskiy.com/";
            
            var extractor = new MetaExtractor.Extractor();
            var metaData = await extractor.Extract(new Uri(url));
        }
    }
}