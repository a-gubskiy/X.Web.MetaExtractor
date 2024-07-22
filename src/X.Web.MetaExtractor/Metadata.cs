using System.Collections.Generic;
using System.Collections.Immutable;
using JetBrains.Annotations;

namespace X.Web.MetaExtractor;

[PublicAPI]
public class Metadata
{
    public Metadata()
    {
        Images = ImmutableArray<string>.Empty;
        Keywords = ImmutableArray<string>.Empty;
        MetaTags = ImmutableArray<KeyValuePair<string, string>>.Empty;
    }
        
    public string Title { get; set; }

    public string Description { get; set; }
        
    public string Raw { get; set; }

    public string Url { get; set; }
        
    public string Language { get; set; }

    public IReadOnlyCollection<string> Images { get; set; }
        
    public IReadOnlyCollection<string> Keywords { get; set; }
        
    public IReadOnlyCollection<KeyValuePair<string, string>> MetaTags { get; set; }

    public override string ToString() => $"{Title}\r\n{Description}\r\n{Url}";
}