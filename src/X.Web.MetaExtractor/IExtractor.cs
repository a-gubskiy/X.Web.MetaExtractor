using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace X.Web.MetaExtractor;

[PublicAPI]
public interface IExtractor
{
    Task<Metadata> ExtractAsync(Uri uri);
        
    Metadata Extract(Uri uri);
}