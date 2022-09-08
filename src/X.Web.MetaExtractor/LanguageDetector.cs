﻿using HtmlAgilityPack;
using JetBrains.Annotations;

namespace X.Web.MetaExtractor;

[PublicAPI]
public class FakeLanguageDetector : ILanguageDetector
{
    public string GetHtmlPageLanguage(string html) => string.Empty;
}

[PublicAPI]
public class LanguageDetector : ILanguageDetector
{
    public virtual string GetHtmlPageLanguage(string html)
    {
        var document = new HtmlDocument();

        try
        {
            document.LoadHtml(html);
                
            return document.DocumentNode.SelectSingleNode("//html")?.Attributes["lang"]?.Value?.ToLower();
        }
        catch
        {
            return string.Empty;
        }
    }
}