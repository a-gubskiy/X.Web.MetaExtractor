using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace X.Web.MetaExtractor;

[PublicAPI]
public interface IPageContentLoader
{
    /// <summary>
    /// Load html
    /// </summary>
    /// <param name="uri">Page url</param>
    /// <returns></returns>
    string LoadPageContent(Uri uri);
        
    /// <summary>
    /// Load html
    /// </summary>
    /// <param name="uri">Page url</param>
    /// <returns></returns>
    Task<string> LoadPageContentAsync(Uri uri);
}