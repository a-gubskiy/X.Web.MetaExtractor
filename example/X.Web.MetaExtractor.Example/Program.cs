using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace X.Web.MetaExtractor.Example;

class Program
{
    static Task Main(string[] args)
    {
        Console.Clear();

        IReadOnlyCollection<Uri> linksTemplate = new List<Uri>
        {
            new Uri("https://diepresse.com/home/wirtschaft/unternehmen/5399476/TeslaChef-Elon-Musk_Das-AutoGeschaeft-ist-die-Hoelle"),
            new Uri("https://andrew.gubskiy.com/"),
            new Uri("https://devdigest.today/post/458"),
            new Uri("https://blogs.msdn.microsoft.com/dotnet/2018/04/11/announcing-net-core-2-1-preview-2/"),
            new Uri("https://github.com/dotnet/corefx/milestone/12"),
            new Uri("https://stackoverflow.com/questions/49790807/can-net-core-1-1-4-run-net-standard-2"),
            new Uri("https://dotnetcoretutorials.com"),
            new Uri("https://softwareengineering.stackexchange.com/questions/305933/json-api-specification-when-do-i-need-to-return-a-404-not-found"),
            new Uri("https://devdigest.today/post/469"),
            new Uri("https://diepresse.com/home/panorama/wien/5386805/Polizist-attackiert_Parlament-verstaerkt-Bewachung"),
            new Uri("https://www.diepresse.com/5748483/thiem-unterliegt-bei-atp-cup-gegen-den-polen-hurkacz")
        };

        var links = new List<Uri>();

        for (var i = 0; i < 1; i++)
        {
            links.AddRange(Generate(linksTemplate));
        }

        var extractor = new Extractor();

        var collection = new BlockingCollection<Metadata>();

        Parallel.ForEach(links, async Task<bool>(uri, state) =>
        {
            Console.WriteLine($"Start extracting {uri}");

            try
            {
                var metadata = await extractor.ExtractAsync(uri);
                collection.Add(metadata);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Url: {uri}. Message: {ex.Message}");
            }

            return true;
        });

        foreach (var m in collection)
        {
            Console.WriteLine($"{m.Title}, {m.Description}");
        }

        Console.Write("OK");
        Console.ReadKey();
        
        return Task.CompletedTask;
    }

    private static IReadOnlyCollection<Uri> Generate(IReadOnlyCollection<Uri> links)
    {
        var result = new List<Uri>();

        foreach (var link in links)
        {
            result.Add(new Uri($"{link}?cb={Guid.NewGuid()}"));
        }

        return result;
    }
}