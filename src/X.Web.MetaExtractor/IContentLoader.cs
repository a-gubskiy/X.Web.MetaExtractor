using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace X.Web.MetaExtractor;

/// <summary>
/// Defines an interface for loading web content from specified URIs.
/// </summary>
/// <remarks>
/// This interface provides methods for retrieving raw content from web pages asynchronously
/// with optional cancellation support. Implementations should handle HTTP requests and
/// return the raw string content of the requested resource.
/// </remarks>
[PublicAPI]
public interface IContentLoader
{
    /// <summary>
    /// Loads the content of a web page from the specified URI with cancellation support.
    /// </summary>
    /// <param name="uri">The URI of the web page to load.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the page content as a string.
    /// </returns>
    Task<string> Load(Uri uri, CancellationToken cancellationToken);

    /// <summary>
    /// Loads the content of a web page from the specified URI.
    /// </summary>
    /// <param name="uri">The URI of the web page to load.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the page content as a string.
    /// </returns>
    Task<string> Load(Uri uri) => Load(uri, CancellationToken.None);
}