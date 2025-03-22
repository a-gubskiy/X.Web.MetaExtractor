using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using JetBrains.Annotations;

namespace X.Web.MetaExtractor;

public record Content
{
    /// <summary>
    /// Gets or initializes the URL of the web content.
    /// </summary>
    public required Uri Url { get; init; }

    /// <summary>
    /// Gets or initializes the raw content from which the metadata was extracted.
    /// </summary>
    public required string Raw { get; init; }
}

/// <summary>
/// Represents metadata extracted from web content, containing properties like title, description,
/// images, keywords, and other meta information.
/// </summary>
[PublicAPI]
public record Metadata : Content
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Metadata"/> class with default empty values.
    /// </summary>
    public Metadata()
    {
        Title = string.Empty;
        Description = string.Empty;
        Raw = string.Empty;
        Language = string.Empty;
        Images = ImmutableArray<string>.Empty;
        Keywords = ImmutableArray<string>.Empty;
        MetaTags = ImmutableArray<KeyValuePair<string, string>>.Empty;
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
    public IReadOnlyCollection<KeyValuePair<string, string>> MetaTags { get; set; }

    /// <summary>
    /// Returns a string representation of the metadata including the title, description, and URL.
    /// </summary>
    /// <returns>A string containing the title, description, and URL, each on a separate line.</returns>
    public override string ToString() => $"{Title}\r\n{Description}\r\n{Url}";
}