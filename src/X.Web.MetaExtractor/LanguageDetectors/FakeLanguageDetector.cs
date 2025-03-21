using JetBrains.Annotations;

namespace X.Web.MetaExtractor.LanguageDetectors;

/// <summary>
/// A fake implementation of the <see cref="ILanguageDetector"/> interface that doesn't perform actual language detection.
/// </summary>
/// <remarks>
/// This class can be used for testing, development, or situations where language detection is not required.
/// Always returns an empty string regardless of input.
/// </remarks>
[PublicAPI]
public class FakeLanguageDetector : ILanguageDetector
{
    /// <summary>
    /// Returns an empty string regardless of the HTML content provided.
    /// </summary>
    /// <param name="html">The HTML content to analyze (ignored in this implementation).</param>
    /// <returns>An empty string.</returns>
    public string GetHtmlPageLanguage(string html) => string.Empty;
}