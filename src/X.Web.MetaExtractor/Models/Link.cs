namespace X.Web.MetaExtractor.Models;

public record Link
{
    public required string Title { get; init; }

    public required string Value { get; init; }
}