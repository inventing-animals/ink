using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Ink.UI.Controls;

/// <summary>
/// Attached property that injects a dim backdrop into the host window's OverlayLayer
/// whenever the target Popup is open. Apply to a Popup element directly - the Popup
/// lives in the main visual tree so OverlayLayer resolution is reliable.
/// </summary>
public static class PopupBackdrop
{
    private static readonly Dictionary<Popup, Border> _backdrops = new();

    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("IsEnabled", typeof(PopupBackdrop));

    /// <summary>
    /// Optional brush override. Defaults to semi-transparent black (#40000000).
    /// </summary>
    public static readonly AttachedProperty<IBrush?> BrushProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("Brush", typeof(PopupBackdrop));

    static PopupBackdrop()
    {
        IsEnabledProperty.Changed.AddClassHandler<Popup>(OnIsEnabledChanged);
        Popup.IsOpenProperty.Changed.AddClassHandler<Popup>(OnIsOpenChanged);
    }

    public static bool GetIsEnabled(AvaloniaObject obj) => obj.GetValue(IsEnabledProperty);
    public static void SetIsEnabled(AvaloniaObject obj, bool value) => obj.SetValue(IsEnabledProperty, value);

    public static IBrush? GetBrush(AvaloniaObject obj) => obj.GetValue(BrushProperty);
    public static void SetBrush(AvaloniaObject obj, IBrush? value) => obj.SetValue(BrushProperty, value);

    private static void OnIsEnabledChanged(Popup popup, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is true && popup.IsOpen)
            ShowBackdrop(popup);
        else if (e.NewValue is false)
            HideBackdrop(popup);
    }

    private static void OnIsOpenChanged(Popup popup, AvaloniaPropertyChangedEventArgs e)
    {
        if (!GetIsEnabled(popup)) return;

        if (e.GetNewValue<bool>())
            ShowBackdrop(popup);
        else
            HideBackdrop(popup);
    }

    private static void ShowBackdrop(Popup popup)
    {
        if (_backdrops.ContainsKey(popup)) return;

        // The Popup element itself is in the main visual tree, so this walk is reliable.
        var overlayLayer = OverlayLayer.GetOverlayLayer(popup);
        if (overlayLayer is null) return;

        var brush = GetBrush(popup) ?? new SolidColorBrush(Color.FromArgb(0x40, 0, 0, 0));
        var backdrop = new Border
        {
            Background = brush,
            ZIndex = -1,
            IsHitTestVisible = false,
        };

        _backdrops[popup] = backdrop;
        overlayLayer.Children.Add(backdrop);
    }

    private static void HideBackdrop(Popup popup)
    {
        if (!_backdrops.TryGetValue(popup, out var backdrop)) return;
        (backdrop.Parent as Panel)?.Children.Remove(backdrop);
        _backdrops.Remove(popup);
    }
}
