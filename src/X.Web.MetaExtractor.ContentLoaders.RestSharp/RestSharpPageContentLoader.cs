using System;
using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;
using RestSharp;

namespace X.Web.MetaExtractor.ContentLoaders.RestSharp;

[PublicAPI]
public class RestSharpPageContentLoader : IPageContentLoader
{
    public async Task<string> LoadPageContentAsync(Uri uri)
    {
        if (uri == null)
        {
            throw new ArgumentNullException(nameof(uri));
        }

        var client = new RestClient();
        var request = new RestRequest(uri, Method.Get);

        var response = await client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new HttpRequestException($"Error fetching content from {uri}. Status code: {response.StatusCode}");
        }

        return response.Content;
    }
}