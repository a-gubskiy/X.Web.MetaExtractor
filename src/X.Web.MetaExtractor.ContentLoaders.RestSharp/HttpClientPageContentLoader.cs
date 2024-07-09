using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace X.Web.MetaExtractor.ContentLoaders.RestSharp;

[PublicAPI]
public class HttpClientPageContentLoader : IPageContentLoader
{
    public Task<string> LoadPageContentAsync(Uri uri)
    {
        throw new NotImplementedException();
    }
}