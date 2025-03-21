using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace X.Web.MetaExtractor;

/// <summary>
/// Interface for extracting metadata from web pages at specified URIs.
/// </summary>
[PublicAPI]
public interface IExtractor
{
    /// <summary>
    /// Extracts metadata from a web page at the specified URI.
    /// </summary>
    /// <param name="uri">The URI of the web page to extract metadata from.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the extracted metadata.
    /// </returns>
    Task<Metadata> Extract(Uri uri) => Extract(uri, CancellationToken.None);
    
    /// <summary>
    /// Extracts metadata from a web page at the specified URI with cancellation support.
    /// </summary>
    /// <param name="uri">The URI of the web page to extract metadata from.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the extracted metadata.
    /// </returns>
    Task<Metadata> Extract(Uri uri, CancellationToken cancellationToken);
}