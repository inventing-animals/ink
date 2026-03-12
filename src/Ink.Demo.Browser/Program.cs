using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Ink.Demo;
using Ink.Demo.Browser;
using Ink.Platform.Browser.Routing;

internal sealed partial class Program
{
    private static Task Main(string[] args)
    {
        App.RouterFactory       = () => new BrowserHistoryRouter();
        App.WindowServiceFactory = () => new BrowserWindowService();

        return BuildAvaloniaApp()
            .WithInterFont()
            .StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}
