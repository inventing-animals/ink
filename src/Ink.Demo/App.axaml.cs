using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Ink.Demo.ViewModels;
using Ink.Demo.Views;
using Ink.Platform.Routing;
using Ink.UI.Controls;
using Ink.UI.Platform;

namespace Ink.Demo;

public partial class App : Application
{
    /// <summary>
    /// Set by the platform entry point before the app starts to provide the correct <see cref="IRouter"/> implementation.
    /// Defaults to <see cref="InMemoryRouter"/> if not set.
    /// </summary>
    public static Func<IRouter> RouterFactory { get; set; } = () => new InMemoryRouter("/buttons");

    /// <summary>
    /// Optionally set by the platform entry point to override the default <see cref="IWindowService"/>.
    /// When null the app selects an appropriate default: <see cref="DesktopWindowService"/> on desktop,
    /// <see cref="DrawerWindowService"/> on mobile/web.
    /// </summary>
    public static Func<IWindowService>? WindowServiceFactory { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit.
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();

            var mainWindow = new MainWindow();
            var appState   = new AppState(RouterFactory(), new DesktopTabbedWindowService(mainWindow), new ThemeService());
            var mainView    = new MainView { DataContext = new MainViewModel(appState) };

            mainWindow.MainContent = mainView;
            desktop.MainWindow = mainWindow;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            MainView? view = null;

            // Browser: use the factory (BrowserWindowService opens a new tab).
            // Mobile: DrawerWindowService shows a bottom-sheet overlay.
            IWindowService windowService = OperatingSystem.IsBrowser()
                ? (WindowServiceFactory?.Invoke()
                    ?? new DrawerWindowService(() => view is not null ? TopLevel.GetTopLevel(view) : null))
                : new DrawerWindowService(() => view is not null ? TopLevel.GetTopLevel(view) : null);

            var appState = new AppState(RouterFactory(), windowService, new ThemeService());
            view = new MainView { DataContext = new MainViewModel(appState) };

            singleViewPlatform.MainView = OperatingSystem.IsBrowser()
                ? new WebWindow    { Content = view }
                : new MobileWindow { Content = view };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}