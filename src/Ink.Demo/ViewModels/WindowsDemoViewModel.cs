using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Ink.Demo.Views;
using Ink.UI.Platform;

namespace Ink.Demo.ViewModels;

public partial class WindowsDemoViewModel : ViewModelBase
{
    private readonly IWindowService _windows;

    public WindowsDemoViewModel(IWindowService windows)
    {
        _windows = windows;
    }

    [RelayCommand]
    private async Task OpenWindow()
    {
        IWindowHandle? handle = null;
        var vm = new SecondaryWindowViewModel(() => handle?.Close());

        handle = await _windows.OpenAsync(
            () => new SecondaryWindowView { DataContext = vm },
            new WindowOptions { Title = "Detail View", Width = 440, Height = 300, Url = "/windows" });
    }

    [RelayCommand]
    private async Task OpenTab()
    {
        IWindowHandle? handle = null;
        var vm = new SecondaryWindowViewModel(() => handle?.Close());

        handle = await _windows.OpenTabAsync(
            () => new SecondaryWindowView { DataContext = vm },
            new WindowOptions { Title = "Detail View", Url = "/windows" });
    }
}
