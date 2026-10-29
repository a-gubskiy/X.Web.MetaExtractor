# X.Web.MetaExtractor

[![Sponsor on GitHub](https://img.shields.io/badge/Sponsor_on_GitHub-ff7f00?logo=github&logoColor=white&style=for-the-badge)](https://github.com/sponsors/a-gubskiy)
[![Subscribe on X](https://img.shields.io/badge/Subscribe_on_X-000000?logo=x&logoColor=white&style=for-the-badge)](https://x.com/andrew_gubskiy)
[![NuGet Downloads](https://img.shields.io/nuget/dt/X.Web.MetaExtractor?style=for-the-badge&label=NuGet%20Downloads&color=004880&logo=nuget&logoColor=white)](https://www.nuget.org/packages/X.Web.MetaExtractor)

**X.Web.MetaExtractor** is a powerful library that allows you to extract meta information from any web page URL. It provides a variety of content loaders to handle HTTP requests using different libraries.

## Breaking Changes

### 3.0

- **Content loader packages are discontinued**: `X.Web.MetaExtractor.ContentLoaders.Flurl`, `X.Web.MetaExtractor.ContentLoaders.FsHttp` and `X.Web.MetaExtractor.ContentLoaders.RestSharp` are no longer published. Use the built-in `HttpClientContentLoader` or implement `IContentLoader` yourself (see [Custom Content Loader](#custom-content-loader)).
- **Target frameworks**: the package now targets `net8.0` and `net10.0`.
- **`HttpClientFactory` no longer shares clients between instances**: keep one instance for the lifetime of the application, or use `AddMetaExtractor()` with dependency injection.

### 2.x

- **Metadata class was changed**: The `Content` field has been removed from the `Metadata` class. Ensure to update your code to reflect this change if you were using the `Content` field.
- **Description Extraction Logic**: The `Extractor` class now only extracts the description from meta tags, without attempting to parse the content of the page.
- **New WebPage Model**: The library now returns a `WebPage` model with comprehensive information including links found on the page.
- **Link Extraction**: Added support for extracting and processing all hyperlinks from web pages.

## Features

- Extract meta information from any web page URL.
- Extract and process hyperlinks from web pages.
- Built-in `HttpClient`-based content loader with configurable timeout and User-Agent.
- Dependency injection support via `services.AddMetaExtractor()`.
- Pluggable content loading through `IContentLoader`.
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

IExtractor extractor = new Extractor("https://example.com/example.jpg");

// Extract information from a URL
var webPage = await extractor.Extract(new Uri("https://example.com"), CancellationToken.None);

// Display the extracted information
Console.WriteLine($"Title: {webPage.Title}");
Console.WriteLine($"Description: {webPage.Description}");
Console.WriteLine($"Keywords: {string.Join(", ", webPage.Keywords)}");
Console.WriteLine($"Language: {webPage.Language}");

// Process links
Console.WriteLine($"Found {webPage.Links.Count} links:");

foreach (var link in webPage.Links)
{
    Console.WriteLine($"- {link.Title}: {link.Value}");
}
```

### Dependency Injection

```csharp
builder.Services.AddMetaExtractor(options =>
{
    options.DefaultImage = "https://example.com/example.jpg";
    options.MaxDescriptionLength = 500;
    options.ContentLoader.Timeout = TimeSpan.FromSeconds(10);
    options.ContentLoader.UserAgent = "MyApp/1.0";
});

// Then inject IExtractor wherever you need it
public class PreviewService(IExtractor extractor) { ... }
```

`AddMetaExtractor()` registers a named client through `IHttpClientFactory`, so connection pooling and DNS refresh are handled by .NET. To use your own loader, register `IContentLoader` before calling `AddMetaExtractor()`.

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

### HttpClient

`HttpClientContentLoader` is the default loader and is included in the main package. Without dependency injection it reuses a single shared `HttpClient`. To change its settings, pass your own factory and keep it for the lifetime of the application:

```csharp
var factory = new HttpClientFactory(new HttpClientContentLoaderOptions
{
    Timeout = TimeSpan.FromSeconds(10)
});

IExtractor extractor = new Extractor(string.Empty, new HttpClientContentLoader(factory), new LanguageDetector());
```

### Custom Content Loader

To load pages with another HTTP library, implement `IContentLoader`. For example, with Flurl:

```csharp
public class FlurlContentLoader : IContentLoader
{
    public Task<string> Load(Uri uri, CancellationToken cancellationToken) =>
        uri.ToString().GetStringAsync(cancellationToken: cancellationToken);
}
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/a-gubskiy/X.Web.MetaExtractor/blob/master/LICENSE) file for more details.
