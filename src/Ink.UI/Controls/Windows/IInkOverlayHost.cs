namespace Ink.UI.Controls;

/// <summary>
/// Implemented by Ink window roots (DesktopWindow, WebWindow, MobileWindow).
/// Controls call InkOverlay.Show/Hide to request the window-level dim backdrop.
/// </summary>
public interface IInkOverlayHost
{
    /// <summary>Increment the overlay request count; shows the dim backdrop if it was hidden.</summary>
    void ShowOverlay();

    /// <summary>Decrement the overlay request count; hides the dim backdrop when it reaches zero.</summary>
    void HideOverlay();
}
