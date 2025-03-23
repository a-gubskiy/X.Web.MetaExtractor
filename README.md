# X.Web.MetaExtractor
[![NuGet version](https://badge.fury.io/nu/X.Web.MetaExtractor.svg)](https://badge.fury.io/nu/X.Web.MetaExtractor)
[![Twitter URL](https://img.shields.io/twitter/url/https/twitter.com/andrew_gubskiy.svg?style=social&label=Follow%20me!)](https://twitter.com/intent/user?screen_name=andrew_gubskiy)

**X.Web.MetaExtractor** is a powerful library that allows you to extract meta information from any web page URL. It provides a variety of content loaders to handle HTTP requests using different libraries.

## Breaking Changes

- **Metadata class was changed**: The `Content` field has been removed from the `Metadata` class. Ensure to update your code to reflect this change if you were using the `Content` field.
- **Description Extraction Logic**: The `Extractor` class now only extracts the description from meta tags, without attempting to parse the content of the page.
- **New WebPage Model**: The library now returns a `WebPage` model with comprehensive information including links found on the page.
- **Link Extraction**: Added support for extracting and processing all hyperlinks from web pages.

## Features

- Extract meta information from any web page URL.
- Extract and process hyperlinks from web pages.
- Support for multiple HTTP libraries:
  - Flurl
  - FsHttp
  - RestSharp
- Detect the language of the page content.

## Installation

To install the library, use the following command:

```bash
dotnet add package X.Web.MetaExtractor
```

## Usage

Here is a basic example of how to use the `X.Web.MetaExtractor` library:

```csharp
using X.Web.MetaExtractor;
using X.Web.MetaExtractor.ContentLoaders;
using X.Web.MetaExtractor.LanguageDetectors;

// Create instances of the necessary components
IContentLoader contentLoader = new FlurlContentLoader();
ILanguageDetector languageDetector = new LanguageDetector();
string defaultImage = "https://example.com/example.jpg";

// Create an instance of the Extractor
IExtractor extractor = new Extractor(defaultImage, contentLoader, languageDetector);

// Extract information from a URL
var webPage = await extractor.Extract(new Uri("https://example.com"), CancellationToken.None);

// Display the extracted information
Console.WriteLine($"Title: {webPage.Title}");
Console.WriteLine($"Description: {webPage.Description}");
Console.WriteLine($"Keywords: {webPage.Keywords}");
Console.WriteLine($"Language: {webPage.Language}");

// Process links
if (webPage.Links != null)
{
    Console.WriteLine($"Found {webPage.Links.Count} links:");
    foreach (var link in webPage.Links)
    {
        Console.WriteLine($"- {link.Title}: {link.Value}");
    }
}
```

## Interfaces and Classes

### IExtractor

`IExtractor` defines the interface for extracting web page information, returning a comprehensive `WebPage` model.

### ILanguageDetector

`ILanguageDetector` defines the interface for detecting the language of the page content.

### IContentLoader

`IContentLoader` defines the interface for loading the content of a web page asynchronously.

### WebPage

`WebPage` is the main model containing extracted information from a web page, including metadata, links, and source information.

### Link

`Link` is a record that represents a hyperlink extracted from HTML content with Title and Value properties.

### Source

`Source` is a record that contains information about the origin of web content, including the original URL and raw page content.

## Extractors

The library architecture supports multiple specialized extractors that work together to build a complete representation of a web page:

* **MetaDocumentExtractor** - Extracts metadata from HTML <meta> tags
* **OpenGraphDocumentExtractor** - Extracts Open Graph protocol metadata
* **TitleDocumentExtractor** - Extracts the page title
* **ImageDocumentExtractor** - Extracts image URLs from the document
* **LinksDocumentExtractor** – Extracts all hyperlinks from HTML documents, converting them to strongly-typed `Link` objects.

## Content Loaders

### Flurl

`X.Web.MetaExtractor.ContentLoaders.Flurl` provides a content loader using the Flurl HTTP library.

### FsHttp

`X.Web.MetaExtractor.ContentLoaders.FsHttp` leverages the FsHttp library to load content.

### HttpClient

`X.Web.MetaExtractor.ContentLoaders.HttpClient` utilizes the HttpClient class to load content.

### RestSharp

`X.Web.MetaExtractor.ContentLoaders.RestSharp` uses the RestSharp library for content loading.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/a-gubskiy/X.Web.MetaExtractor/blob/master/LICENSE) file for more details.