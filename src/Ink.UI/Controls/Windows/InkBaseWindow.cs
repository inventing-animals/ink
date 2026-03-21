using System;
using Avalonia;
using Avalonia.Controls;

namespace Ink.UI.Controls;

/// <summary>
/// Shared base for <see cref="MobileWindow"/> and <see cref="WebWindow"/>.
/// Sets the root content to an overlay-managed panel and exposes
/// <see cref="MainContent"/> for the application view.
/// </summary>
public abstract class InkBaseWindow : UserControl, IInkOverlayHost
{
    /// <summary>Access point for subclasses that need to place structural content in the overlay layer.</summary>
    private protected readonly InkOverlayLayer OverlayLayer = new();

    protected InkBaseWindow()
    {
        Content = OverlayLayer.Root;
    }

    /// <summary>The application view shown inside this window root.</summary>
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
