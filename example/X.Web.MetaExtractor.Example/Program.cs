using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace X.Web.MetaExtractor.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();

            var linksTemplate = new List<Uri>
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
                new Uri("https://diepresse.com/home/panorama/wien/5386805/Polizist-attackiert_Parlament-verstaerkt-Bewachung")
            };

            var links = new List<Uri>();

            for (int i = 0; i < 10; i++)
            {
                links.AddRange(Generate(linksTemplate));
            }

            var extractor = new Extractor("", TimeSpan.FromSeconds(5), false);

            var сollection = new BlockingCollection<Metadata>();

            Parallel.ForEach(links, (uri, state) =>
            {
                Console.WriteLine($"Start extracting {uri}");
                
                var metadata = extractor.Extract(uri);
                сollection.Add(metadata);
                
                Console.WriteLine($"{metadata.Title}");
            });

            foreach (var m in сollection)
            {
                Console.WriteLine($"{m.Title}, {m.Description}");
            }

            Console.Write("OK");
            Console.ReadKey();
        }

        private static IEnumerable<Uri> Generate(List<Uri> links)
        {
            var result = new List<Uri>();

            foreach (var link in links)
            {
                result.Add(new Uri($"{link.ToString()}?cb={Guid.NewGuid()}"));
            }

            return result;
        }
    }
}