# X.Web.MetaExtractor
[![NuGet version](https://badge.fury.io/nu/X.Web.MetaExtractor.svg)](https://badge.fury.io/nu/X.Web.MetaExtractor)
[![Twitter URL](https://img.shields.io/twitter/url/https/twitter.com/andrew_gubskiy.svg?style=social&label=Follow%20me!)](https://twitter.com/intent/user?screen_name=andrew_gubskiy)

**X.Web.MetaExtractor** is a powerful library that allows you to extract meta information from any web page URL. It provides a variety of content loaders to handle HTTP requests using different libraries.

## Breaking Changes

- **Metadata class was changes**: The `Content` field has been removed from the `Metadata` class. Ensure to update your code to reflect this change if you were using the `Content` field.
- **Description Extraction Logic**: The `Extractor` class now only extracts the description from meta tags, without attempting to parse the content of the page. Adjust your implementation if it relied on content parsing for the description.

## Features

- Extract meta information from any web page URL.
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
IPageContentLoader contentLoader = new FlurlPageContentLoader();
ILanguageDetector languageDetector = new LanguageDetector();
string defaultImage = "https://example.com/example.jpg";

// Create an instance of the Extractor
IExtractor extractor = new Extractor(defaultImage, contentLoader, languageDetector);

// Extract meta information from a URL
var metaInfo = await extractor.ExtractAsync( new Uri("https://example.com"));

// Display the extracted meta information
Console.WriteLine($"Title: {metaInfo.Title}");
Console.WriteLine($"Description: {metaInfo.Description}");
Console.WriteLine($"Keywords: {metaInfo.Keywords}");
Console.WriteLine($"Language: {metaInfo.Language}");
```

## Interfaces and Classes

### IExtractor

`IExtractor` defines the interface for extracting meta information.

### ILanguageDetector

`ILanguageDetector` defines the interface for detecting the language of the page content.

### IPageContentLoader

`IPageContentLoader` defines the interface for loading the content of a web page.

### Metadata

`Metadata` is a class that holds the meta information of a web page, including the title, description, keywords, and language.

## Content Loaders

### Flurl

`X.Web.MetaExtractor.ContentLoaders.Flurl` provides a content loader using the Flurl HTTP library, enabling efficient and fluent HTTP request handling for meta information extraction from any page URL.

### FsHttp

`X.Web.MetaExtractor.ContentLoaders.FsHttp` leverages the FsHttp library to load content, facilitating robust and type-safe HTTP request execution for extracting meta information from any page URL.

### HttpClient

`X.Web.MetaExtractor.ContentLoaders.HttpClient` utilizes the HttpClient class to load content, offering a flexible and reliable approach to perform HTTP requests for meta information extraction from any page URL.

### RestSharp

`X.Web.MetaExtractor.ContentLoaders.RestSharp` uses the RestSharp library for content loading, providing an intuitive and powerful way to handle HTTP requests for extracting meta information from any page URL.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/ernado-x/X.Web.MetaExtractor/blob/master/LICENSE) file for more details.
