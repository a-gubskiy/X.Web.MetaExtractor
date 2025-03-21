using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace X.Web.MetaExtractor;

[PublicAPI]
public interface IExtractor
{
    Task<Metadata> Extract(Uri uri) => Extract(uri, CancellationToken.None);
    
    Task<Metadata> Extract(Uri uri, CancellationToken cancellationToken);
}