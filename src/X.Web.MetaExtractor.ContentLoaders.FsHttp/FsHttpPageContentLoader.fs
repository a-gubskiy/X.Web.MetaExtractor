namespace X.Web.MetaExtractor.ContentLoaders.FsHttp

open System
open System.Threading.Tasks
open FsHttp
open JetBrains.Annotations

[<PublicAPI>]
type FsHttpPageContentLoader() =
    interface X.Web.MetaExtractor.IPageContentLoader with
        member _.LoadPageContentAsync(uri: Uri) : Task<string> =
            task {
                let url = uri.ToString()

                let! (response: Response) = http { GET url } |> Request.sendAsync

                let! (content: string) = response.content.ReadAsStringAsync()

                return content
            }
