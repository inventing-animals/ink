using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ink.Platform.Routing;

namespace Ink.Demo.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IRouter _router = new InMemoryRouter("/");

    [ObservableProperty]
    private string _greeting = "Welcome to Ink!";

    [ObservableProperty]
    private string _currentPath = "/";

    [ObservableProperty]
    private string _currentSegments = "(root)";

    [ObservableProperty]
    private string _currentQuery = "";

    [ObservableProperty]
    private string _currentFragment = "";

    public MainViewModel()
    {
        _router.LocationChanged += (_, location) => UpdateLocation(location);
    }

    [RelayCommand]
    private void NavigateTo(string path) => _router.Navigate(path);

    [RelayCommand]
    private void Back() => _router.Back();

    [RelayCommand]
    private void Forward() => _router.Forward();

    private void UpdateLocation(ILocation location)
    {
        CurrentPath = location.Path;
        CurrentSegments = location.Segments.Count > 0
            ? string.Join(" / ", location.Segments)
            : "(root)";
        CurrentQuery = location.Query ?? "";
        CurrentFragment = location.Fragment ?? "";
    }
}
