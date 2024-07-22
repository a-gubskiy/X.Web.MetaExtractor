using HtmlAgilityPack;
using JetBrains.Annotations;

namespace X.Web.MetaExtractor.LanguageDetectors;

[PublicAPI]
public class LanguageDetector : ILanguageDetector
{
    public virtual string GetHtmlPageLanguage(string html)
    {
        var document = new HtmlDocument();

        try
        {
            document.LoadHtml(html);

            var language = document.DocumentNode.SelectSingleNode("//html")?.Attributes["lang"]?.Value?.ToLower();

            return language ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}