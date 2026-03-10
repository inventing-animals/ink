using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ink.Platform.Routing;

namespace Ink.Demo.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly AppState _appState;

    [ObservableProperty]
    private ViewModelBase? _currentPage;

    public IRouter Router => _appState.Router;

    public MainViewModel(AppState appState)
    {
        _appState = appState;
        _appState.Router.LocationChanged += (_, location) => UpdatePage(location);
        UpdatePage(_appState.Router.Current);
    }

    [RelayCommand]
    private void Navigate(string path) => _appState.Router.Navigate(path);

    private void UpdatePage(ILocation location)
    {
        CurrentPage = location.Segments.FirstOrDefault() switch
        {
            "buttons" => new ButtonsViewModel(),
            "palette" => new PaletteViewModel(),
            "router" => new RouterDemoViewModel(_appState.Router),
            _ => new ButtonsViewModel(),
        };
    }
}
