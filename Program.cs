using JobFetcherManager;
using Menus;

class Program
{
    static async Task Main(string[] args)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        var adzuna = new AdzunaJobProvider();
        var remotive = new RemotiveJobProvider();

        var providers = new List<IJobProvider>
        {
            adzuna,
            remotive
        };

        var menu = new MenuManager(providers);
        await menu.LoopAsync();
    }
}