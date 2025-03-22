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
var metaInfo = await extractor.Extract(uri);

Console.Clear();

// Display the extracted meta information
Console.WriteLine($"Url: {metaInfo.Url}");
Console.WriteLine($"Title: {metaInfo.Title}");
Console.WriteLine($"Description: {metaInfo.Description}");
Console.WriteLine($"Keywords: {string.Join(", ", metaInfo.Keywords)}");
Console.WriteLine($"Image: {metaInfo.Images.FirstOrDefault()}");
Console.WriteLine($"Language: {metaInfo.Language}");