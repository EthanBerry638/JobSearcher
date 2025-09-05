using JobFetcherManager;
using Menus;

class Program
{
    static async Task Main(string[] args)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        
        var provider = new AdzunaJobProvider();
        var menu = new MenuManager(provider);
        await menu.LoopAsync();
    }
}