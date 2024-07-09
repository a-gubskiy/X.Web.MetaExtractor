using System;
using System.Threading.Tasks;
using Flurl.Http;
using JetBrains.Annotations;

namespace X.Web.MetaExtractor.ContentLoaders.Flurl;

[PublicAPI]
public class FlurlPageContentLoader : IPageContentLoader
{
    public async Task<string> LoadPageContentAsync(Uri uri)
    {
        var html = await uri.ToString().GetStringAsync();
        
        return html;
    }
}