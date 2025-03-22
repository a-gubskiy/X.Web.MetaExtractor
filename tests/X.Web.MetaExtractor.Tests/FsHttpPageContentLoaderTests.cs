using System;
using System.Threading.Tasks;
using X.Web.MetaExtractor.ContentLoaders.FsHttp;
using Xunit;

namespace X.Web.MetaExtractor.Tests;

public class FsHttpPageContentLoaderTests
{
    [Fact]
    public async Task Test()
    {
        IContentLoader contentLoader = new FsHttpPageContentLoader();
        
        var task1 = contentLoader.LoadPageContent(new Uri("http://andrew.gubskiy.com/"));
        var task2 = contentLoader.LoadPageContent(new Uri("http://torf.tv/"));
        var task3 = contentLoader.LoadPageContent(new Uri("http://torf.tv/video/IraSkladPortrait"));
        var task4 = contentLoader.LoadPageContent(new Uri("https://en.wikipedia.org/wiki/Oceanic_whitetip_shark"));
        var task5 = contentLoader.LoadPageContent(new Uri("https://devdigest.today/platform/"));

        var all = await Task.WhenAll(task1, task2, task3, task4, task5);

        var html1 = all[0];
        var html2 = all[1];
        var html3 = all[2];
        var html4 = all[3];
        var html5 = all[4];
        
        Assert.NotNull(html1);
        Assert.NotNull(html2);
        Assert.NotNull(html3);
        Assert.NotNull(html4);
        Assert.NotNull(html5);
    }
}