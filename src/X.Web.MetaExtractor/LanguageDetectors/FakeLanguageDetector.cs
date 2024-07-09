using JetBrains.Annotations;

namespace X.Web.MetaExtractor.LanguageDetectors;

[PublicAPI]
public class FakeLanguageDetector : ILanguageDetector
{
    public string GetHtmlPageLanguage(string html) => string.Empty;
}