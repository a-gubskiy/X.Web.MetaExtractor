using System;

namespace X.Web.MetaExtractor;

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