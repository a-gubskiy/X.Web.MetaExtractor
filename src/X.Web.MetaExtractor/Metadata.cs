using System.Collections.Generic;

namespace X.Web.MetaExtractor
{
    public class Metadata
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public string Url { get; set; }

        public IEnumerable<string> Image { get; set; }

        public string Type { get; set; }

        public override string ToString() => $"{Title}\r\n{Description}\r\n{Url}";
    }
}
