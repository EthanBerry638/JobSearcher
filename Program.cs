using JobFetcherManager;
using Menus;
using Providers;

class Program
{
    static async Task Main(string[] args)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        var adzuna = new AdzunaJobProvider();
        var remotive = new RemotiveJobProvider();
        var provider = new MultiJobProvider(adzuna, remotive);

        var menu = new MenuManager(provider);
        await menu.LoopAsync();
    }
}