using System;
using System.Net.Http;
using X.Web.MetaExtractor.ContentLoaders.HttpClient;

namespace X.Web.MetaExtractor;

[Obsolete("Use X.Web.MetaExtractor.ContentLoaders.HttpClient.HttpClientPageContentLoader instead")]
public class PageContentLoader : HttpClientPageContentLoader
{
    public PageContentLoader(IHttpClientFactory httpClientFactory)
        : base(httpClientFactory)
    {
    }

    public PageContentLoader(IHttpClientFactory httpClientFactory, string httpClientName)
        : base(httpClientFactory, httpClientName)
    {
    }

    public PageContentLoader()
        : base()
    {
    }
}