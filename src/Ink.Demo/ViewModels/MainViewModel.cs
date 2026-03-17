using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ink.Platform.Routing;

namespace Ink.Demo.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly AppState _appState;

    [ObservableProperty]
    private object? _currentPage;

    public IRouter Router => _appState.Router;

    public MainViewModel(AppState appState)
    {
        _appState = appState;
        _appState.Theme.ThemeChanged += (_, _) => OnPropertyChanged(nameof(ThemeLabel));
        _appState.Router.LocationChanged += (_, location) => UpdatePage(location);
        UpdatePage(_appState.Router.Current);
    }

    public string GitRevision { get; } =
        typeof(MainViewModel).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion
            .Split('+').LastOrDefault() ?? "unknown";

    public string ThemeLabel =>
        _appState.Theme.Current == Avalonia.Styling.ThemeVariant.Dark ? "Light mode" : "Dark mode";

    [RelayCommand]
    private void Navigate(string path) => _appState.Router.Navigate(path);

    [RelayCommand]
    private void ToggleTheme() => _appState.Theme.Toggle();

    private void UpdatePage(ILocation location)
    {
        CurrentPage = location.Segments.FirstOrDefault() switch
        {
            "buttons"     => new ButtonsViewModel(),
            "palette"     => new PaletteViewModel(),
            "router"      => new RouterDemoViewModel(_appState.Router),
            "windows"     => new WindowsDemoViewModel(_appState.Windows),
            "datagrid"    => new DataGridDemoViewModel(),
            "checkbox"    => new CheckboxViewModel(),
            "radiobutton" => new RadioButtonViewModel(),
            "calendar"    => new CalendarViewModel(),
            "combobox"    => new ComboBoxViewModel(),
            "listbox"     => new ListBoxViewModel(),
            "textbox"     => new TextBoxViewModel(),
            "toggle"      => new ToggleViewModel(),
            "slider"      => new SliderViewModel(),
            "progressbar" => new ProgressBarViewModel(),
            "scrollview"  => new ScrollViewViewModel(),
            _ => new ButtonsViewModel(),
        };
    }
}
