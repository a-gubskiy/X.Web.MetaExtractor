using System;

namespace X.Web.MetaExtractor.Models;

/// <summary>
/// Represents the source information for web content, including the original URL and raw page content.
/// </summary>
public record Source
{
    /// <summary>
    /// URL of the web content.
    /// </summary>
    public required Uri Url { get; init; }

    /// <summary>
    /// Raw page content
    /// </summary>
    public required string Raw { get; init; }
}