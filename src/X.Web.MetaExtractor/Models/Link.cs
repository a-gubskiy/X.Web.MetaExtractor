namespace X.Web.MetaExtractor.Models;

/// <summary>
/// Represents a hyperlink extracted from HTML content.
/// </summary>
/// <remarks>
/// This record is used to store the essential information about links
/// found in web documents during the extraction process.
/// </remarks>
public record Link
{
    /// <summary>
    /// Gets or initializes the title or text content of the link.
    /// </summary>
    /// <value>The display text of the hyperlink.</value>
    public required string Title { get; init; }

    /// <summary>
    /// Gets or initializes the URL or href value of the link.
    /// </summary>
    /// <value>The destination URL of the hyperlink.</value>
    public required string Value { get; init; }
}