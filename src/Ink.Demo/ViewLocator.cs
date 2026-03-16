using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Ink.Demo.ViewModels;
using Ink.Demo.Views;

namespace Ink.Demo;

/// <summary>
/// Maps view model types to view factories without reflection, safe for IL trimming and AOT.
/// </summary>
public class ViewLocator : IDataTemplate
{
    private static readonly Dictionary<System.Type, System.Func<Control>> Map = new()
    {
        [typeof(MainViewModel)] = () => new MainView(),
        [typeof(ButtonsViewModel)] = () => new ButtonsView(),
        [typeof(PaletteViewModel)] = () => new PaletteView(),
        [typeof(RouterDemoViewModel)] = () => new RouterDemoView(),
        [typeof(WindowsDemoViewModel)] = () => new WindowsDemoView(),
        [typeof(DataGridDemoViewModel)] = () => new DataGridDemoView(),
    };

    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        if (Map.TryGetValue(param.GetType(), out var factory))
            return factory();

        return new TextBlock { Text = "Not Found: " + param.GetType().FullName };
    }

    public bool Match(object? data)
    {
        return data is not null && Map.ContainsKey(data.GetType());
    }
}
