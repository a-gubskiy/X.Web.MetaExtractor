using System.Collections.Generic;

namespace X.Web.MetaExtractor
{
    public class Metadata
    {
        public Metadata()
        {
            Images = new List<string>();
            Keywords = new List<string>();
            MetaTags = new List<KeyValuePair<string, string>>();
        }
        
        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }
        
        public string Raw { get; set; }

        public string Url { get; set; }
        
        public string Language { get; set; }

        public IReadOnlyCollection<string> Images { get; set; }
        
        public IReadOnlyCollection<string> Keywords { get; set; }
        
        public IReadOnlyCollection<KeyValuePair<string, string>> MetaTags { get; set; }

        public override string ToString() => $"{Title}\r\n{Description}\r\n{Url}";
    }
}
