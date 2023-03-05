using JetBrains.Annotations;

namespace X.Web.MetaExtractor;

[PublicAPI]
public interface ILanguageDetector
{
    /// <summary>
    /// Return language code, or empty
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    string GetHtmlPageLanguage(string html);
}