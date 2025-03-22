using X.Web.MetaExtractor;
using X.Web.MetaExtractor.ContentLoaders.Flurl;
using X.Web.MetaExtractor.LanguageDetectors;

IContentLoader contentLoader = new FlurlContentLoader();
ILanguageDetector languageDetector = new LanguageDetector();
string defaultImage = "https://example.com/example.jpg";

// Create an instance of the Extractor
IExtractor extractor = new Extractor(defaultImage, contentLoader, languageDetector);

// Extract meta information from a URL
var uri = new Uri("https://andrew.gubskiy.com/content/item/about");
var page = await extractor.Extract(uri);

Console.Clear();

// Display the extracted meta information
Console.WriteLine($"Url: {page.Source!.Url}");
Console.WriteLine($"Title: {page.Title}");
Console.WriteLine($"Description: {page.Description}");
Console.WriteLine($"Keywords: {string.Join(", ", page.Keywords)}");
Console.WriteLine($"Image: {page.Images.FirstOrDefault()}");
Console.WriteLine($"Language: {page.Language}");