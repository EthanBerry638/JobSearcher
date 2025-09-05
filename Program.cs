using JobFetcherManager;
using Menus;

class Program
{
    static async Task Main(string[] args)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        var adzuna = new AdzunaJobProvider();
        var remotive = new RemotiveJobProvider();
        var jooble = new JoobleProvider();

        var providers = new List<IJobProvider>
        {
            adzuna,
            remotive,
            jooble
        };

        var menu = new MenuManager(providers);
        await menu.LoopAsync();
    }
}