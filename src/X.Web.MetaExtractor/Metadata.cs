using System.Collections.Generic;

namespace X.Web.MetaExtractor
{
    public class Metadata
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public string Url { get; set; }
        
        public string Language { get; set; }

        public IReadOnlyCollection<string> Images { get; set; }

        public override string ToString() => $"{Title}\r\n{Description}\r\n{Url}";
    }
}
