using System;
using System.Threading.Tasks;

namespace X.Web.MetaExtractor
{
    public interface IExtractor
    {
        Task<Metadata> ExtractAsync(Uri uri);
        
        [Obsolete]
        Metadata Extract(Uri uri);
    }
}