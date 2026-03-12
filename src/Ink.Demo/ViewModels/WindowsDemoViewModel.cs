using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ink.Demo.Views;
using Ink.UI.Platform;

namespace Ink.Demo.ViewModels;

public partial class WindowsDemoViewModel : ViewModelBase
{
    private readonly IWindowService _windows;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenWindowCommand))]
    private bool _isWindowOpen;

    public WindowsDemoViewModel(IWindowService windows)
    {
        _windows = windows;
    }

    [RelayCommand(CanExecute = nameof(CanOpenWindow))]
    private async Task OpenWindow()
    {
        // Capture handle via closure — safe because OpenAsync returns synchronously,
        // so handle is set before any user interaction can trigger Close().
        IWindowHandle? handle = null;
        var vm = new SecondaryWindowViewModel(() => handle?.Close());

        handle = await _windows.OpenAsync(
            () => new SecondaryWindowView { DataContext = vm },
            new WindowOptions { Title = "Detail View", Width = 440, Height = 300, Url = "/windows" });

        IsWindowOpen = true;
        await handle.WaitForCloseAsync();
        IsWindowOpen = false;
    }

    private bool CanOpenWindow() => !IsWindowOpen;
}
