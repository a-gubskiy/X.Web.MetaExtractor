using System;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using JetBrains.Annotations;

namespace X.Web.MetaExtractor.ContentLoaders.Flurl;

[PublicAPI]
public class FlurlContentLoader : IContentLoader
{
    public async Task<string> Load(Uri uri, CancellationToken cancellationToken)
    {
        var html = await uri.ToString().GetStringAsync(cancellationToken: cancellationToken);
        
        return html;
    }
}