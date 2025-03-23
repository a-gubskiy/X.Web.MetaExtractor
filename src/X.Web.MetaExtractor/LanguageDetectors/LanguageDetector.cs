using HtmlAgilityPack;
using JetBrains.Annotations;

namespace X.Web.MetaExtractor.LanguageDetectors;

/// <summary>
/// Implements the <see cref="ILanguageDetector"/> interface to detect the language of HTML content.
/// </summary>
/// <remarks>
/// This class extracts the language from the "lang" attribute of the HTML tag using HtmlAgilityPack.
/// </remarks>
[PublicAPI]
public class LanguageDetector : ILanguageDetector
{
    /// <summary>
    /// Detects the language of the provided HTML content by extracting the "lang" attribute from the HTML tag.
    /// </summary>
    /// <param name="html">The HTML content to analyze for language detection.</param>
    /// <returns>
    /// A lowercase string representing the detected language code (e.g., "en", "fr") or an empty string
    /// if the language attribute is not found or if an exception occurs during parsing.
    /// </returns>
    public virtual string GetHtmlPageLanguage(string html)
    {
        var document = new HtmlDocument();

        try
        {
            document.LoadHtml(html);

            var node = document.DocumentNode.SelectSingleNode("//html");
            var languageAttribute = node?.Attributes["lang"];
            var language = languageAttribute?.Value?.ToLower();

            return language ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}