using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;

namespace X.Web.MetaExtractor.Models;

/// <summary>
/// Represents metadata extracted from web content, containing properties like title, description,
/// images, keywords, and other meta information.
/// </summary>
[PublicAPI]
public record WebPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebPage"/> class with default empty values.
    /// </summary>
    public WebPage()
    {
        Source = null;
        Title = string.Empty;
        Description = string.Empty;
        Language = string.Empty;
        Images = ImmutableArray<string>.Empty;
        Keywords = ImmutableArray<string>.Empty;
        Metadata = ImmutableArray<KeyValuePair<string, string>>.Empty;
    }

    /// <summary>
    /// Gets or initializes the title of the web content.
    /// </summary>
    public string Title { get; init; }

    /// <summary>
    /// Gets or initializes the description of the web content.
    /// </summary>
    public string Description { get; init; }

    /// <summary>
    /// Gets or initializes the language of the web content.
    /// </summary>
    public string Language { get; init; }

    /// <summary>
    /// Gets or initializes the collection of image URLs found in the web content.
    /// </summary>
    public IReadOnlyCollection<string> Images { get; init; }

    /// <summary>
    /// Gets or initializes the collection of keywords associated with the web content.
    /// </summary>
    public IReadOnlyCollection<string> Keywords { get; init; }

    /// <summary>
    /// Gets or sets the collection of meta tags extracted from the web content.
    /// </summary>
    public IReadOnlyCollection<KeyValuePair<string, string>> Metadata { get; init; }

    /// <summary>
    /// Gets a collection of Open Graph protocol metadata.
    /// </summary>
    /// <remarks>
    /// This property returns all metadata entries with keys that start with "og:" prefix (case-insensitive),
    /// which conform to the Open Graph protocol standard used for social media integration.
    /// </remarks>
    /// <returns>A read-only collection of key-value pairs representing Open Graph metadata.</returns>
    public IReadOnlyCollection<KeyValuePair<string, string>> OpenGraph
    {
        get
        {
            
            
            return Metadata
                .Where(o => o.Key.StartsWith("og:", StringComparison.InvariantCultureIgnoreCase))
                .ToImmutableList();
        }
    }

    public Source? Source { get; init; }
}