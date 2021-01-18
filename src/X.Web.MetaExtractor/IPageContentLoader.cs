using System;
using System.Threading.Tasks;

namespace X.Web.MetaExtractor
{
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
}