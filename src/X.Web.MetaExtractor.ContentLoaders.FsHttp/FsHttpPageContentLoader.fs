namespace X.Web.MetaExtractor.ContentLoaders.FsHttp

open System
open System.Threading
open System.Threading.Tasks
open FsHttp
open JetBrains.Annotations

/// <summary>
/// FsHttpPageContentLoader is a class that implements the IPageContentLoader interface,
/// providing functionality to asynchronously load the content of a web page given its URI.
/// It uses FsHttp for HTTP requests and handles the process in an asynchronous workflow.
/// </summary>
[<PublicAPI>]
type FsHttpPageContentLoader() =
    interface X.Web.MetaExtractor.IContentLoader with
        member _.LoadPageContent(uri: Uri, cancellationToken: CancellationToken) : Task<string> =
            task {
                let url = uri.ToString()

                let! (response: Response) = http { GET url } |> Request.sendAsync

                let! (content: string) = response.content.ReadAsStringAsync(cancellationToken)

                return content
            }
