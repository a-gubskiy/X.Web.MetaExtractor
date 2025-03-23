using JetBrains.Annotations;

namespace X.Web.MetaExtractor;

/// <summary>
/// Interface for detecting the language of HTML page content.
/// </summary>
[PublicAPI]
public interface ILanguageDetector
{
    /// <summary>
    /// Detects and returns the language of the provided HTML content.
    /// </summary>
    /// <param name="html">The HTML content to analyze for language detection.</param>
    /// <returns>A string representing the detected language code (e.g., "en", "fr", "de").</returns>
    string GetHtmlPageLanguage(string html);
}