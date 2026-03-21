using System;
using Avalonia;
using Avalonia.Controls;

namespace Ink.UI.Controls;

/// <summary>
/// Root window for desktop applications built with Ink UI.
/// Use as the top-level <see cref="Window"/> in your desktop entry point.
/// Set <see cref="MainContent"/> (not <see cref="Avalonia.Controls.ContentControl.Content"/>)
/// to place application content inside the overlay-managed panel.
/// </summary>
public class DesktopWindow : Window, IInkOverlayHost
{
    /// <summary>Access point for subclasses that need to place structural content in the overlay layer.</summary>
    private protected readonly InkOverlayLayer OverlayLayer = new();

    public DesktopWindow()
    {
        Content = OverlayLayer.Root;
    }

    /// <summary>The application view shown inside this window.</summary>
    public Control? MainContent
    {
        get => OverlayLayer.Content;
        set => OverlayLayer.Content = value;
    }

    /// <inheritdoc/>
    public void ShowOverlay(Visual registrant, Action onDismiss) => OverlayLayer.Show(registrant, onDismiss);

    /// <inheritdoc/>
    public void HideOverlay(Visual registrant) => OverlayLayer.Hide(registrant);
}
