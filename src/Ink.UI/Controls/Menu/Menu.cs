using Avalonia;

namespace Ink.UI.Controls;

public class Menu : Avalonia.Controls.Menu
{
    public static readonly StyledProperty<bool?> OverlayEnabledProperty =
        AvaloniaProperty.Register<Menu, bool?>(nameof(OverlayEnabled));

    public bool? OverlayEnabled
    {
        get => GetValue(OverlayEnabledProperty);
        set => SetValue(OverlayEnabledProperty, value);
    }
}
