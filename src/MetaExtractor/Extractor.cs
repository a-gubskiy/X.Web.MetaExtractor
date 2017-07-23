using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MetaExtractor
{
    public class Extractor
    {
        public async Task<Metadata> Extract(Uri uri)
        {
            var client = new HttpClient();

            var bytes = await client.GetByteArrayAsync(uri);
            var html = Encoding.UTF8.GetString(bytes);


            throw new NotImplementedException();
        }
    }
}