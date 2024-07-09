using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace X.Web.MetaExtractor;

/// <summary>
/// Cleanup tags from html
/// </summary>
internal static class HtmlCleaner
{
    private static readonly Regex RemoveDoctype = new Regex(@"<!doctype html>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex ReplaceMultipleBreaks = new Regex(@"(<br\s*/?>\s*){3,}", RegexOptions.Compiled);
    private static readonly Regex AddNewlineAfterBreak = new Regex(@"(<br\s*/?>)(?!\n)", RegexOptions.Compiled);
    private static readonly Regex RemoveLongSpaces = new Regex(@"\s{2,}", RegexOptions.Compiled);
    private static readonly Regex TrimLineStartSpaces = new Regex(@"(?m)^\s+", RegexOptions.Compiled);
    private static readonly Regex RemoveLeadingBreaksAndSpaces = new Regex(@"^(<br\s*/?>\s*)+", RegexOptions.Multiline | RegexOptions.Compiled);

    public static string CleanUp(string html)
    {
        var stopwatch = Stopwatch.StartNew();
        
        // Replace newlines with <br /> to standardize the input
        var result = Regex.Replace(html, @"[\r\n]{2,}", "<br />");

        // Remove <!doctype html>
        result = RemoveDoctype.Replace(result, "");

        // Replace multiple <br /> with at most two <br />
        result = ReplaceMultipleBreaks.Replace(result, "<br /><br />");

        // Add a line break after each <br /> tag
        result = AddNewlineAfterBreak.Replace(result, "$1\n");

        // Remove long spaces
        result = RemoveLongSpaces.Replace(result, " ");

        // Trim spaces at the beginning of each line
        result = TrimLineStartSpaces.Replace(result, "");

        // Remove <br /> and spaces if the text begins with them
        result = RemoveLeadingBreaksAndSpaces.Replace(result, "");
        
        stopwatch.Stop();
        
        Trace.WriteLine($"CleanUpText executed in {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"CleanUpText executed in {stopwatch.ElapsedMilliseconds} ms");

        return result;
    }
}