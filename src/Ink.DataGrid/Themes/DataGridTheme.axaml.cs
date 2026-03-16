using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Ink.DataGrid.Themes;

public class DataGridTheme : Styles
{
    public DataGridTheme()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
