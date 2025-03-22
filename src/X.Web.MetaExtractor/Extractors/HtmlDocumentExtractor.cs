using System;
using System.Net;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace X.Web.MetaExtractor.Extractors;

/// <summary>
/// Defines a contract for extracting typed data from HTML documents.
/// </summary>
/// <typeparam name="T">The type of data to be extracted from the HTML document.</typeparam>
public interface IHtmlDocumentExtractor<out T>
{
    /// <summary>
    /// Extracts data of type T from the provided HTML document.
    /// </summary>
    /// <param name="document">The HTML document to process.</param>
    /// <returns>The extracted data of type T.</returns>
    public T? Extract(HtmlDocument document);
}

/// <summary>
/// Base class providing common functionality for HTML document extractors.
/// </summary>
/// <typeparam name="T">The type of data to be extracted from the HTML document.</typeparam>
public abstract class HtmlDocumentExtractor<T> : IHtmlDocumentExtractor<T>
{
    private readonly ILogger _logger;

    public HtmlDocumentExtractor()
        : this(new NullLogger<HtmlDocumentExtractor<T>>())
    {
    }

    public HtmlDocumentExtractor(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Extracts the value of an Open Graph meta tag from the HTML document.
    /// </summary>
    /// <param name="document">The HTML document to extract from.</param>
    /// <param name="name">The Open Graph property name to look for.</param>
    /// <returns>The decoded and trimmed content of the property, or an empty string if not found.</returns>
    protected string ReadOpenGraphProperty(HtmlDocument document, string name)
    {
        var node = document.DocumentNode.SelectSingleNode($"//meta[@property='{name}']");
        var content = node?.Attributes["content"]?.Value ?? string.Empty;
        var result = HtmlDecode(content).Trim();

        return result;
    }

    /// <summary>
    /// Decodes HTML-encoded text to plain text.
    /// </summary>
    /// <param name="text">The HTML-encoded text to decode.</param>
    /// <returns>The decoded text, or an empty string if the input is null or whitespace.</returns>
    protected string HtmlDecode(string text)
    {
        var result = string.IsNullOrWhiteSpace(text)
            ? string.Empty
            : WebUtility.HtmlDecode(text);

        return result;
    }

    /// <summary>
    /// Extracts data of type T from the provided HTML document.
    /// Must be implemented by derived classes.
    /// </summary>
    /// <param name="document">The HTML document to extract data from.</param>
    /// <returns>The extracted data of type T.</returns>
    protected abstract T ExtractInternal(HtmlDocument document);

    
    public T? Extract(HtmlDocument document)
    {
        try
        {
            return ExtractInternal(document);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            return default;
        }
    }
}