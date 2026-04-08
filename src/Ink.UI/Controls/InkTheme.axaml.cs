using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Ink.UI.Controls;

public class InkTheme : Styles
{
    public InkTheme()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
