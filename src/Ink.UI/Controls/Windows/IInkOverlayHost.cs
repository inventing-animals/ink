using System;
using Avalonia;

namespace Ink.UI.Controls;

/// <summary>
/// Implemented by Ink window roots (DesktopWindow, WebWindow, MobileWindow).
/// Controls call InkOverlay.Show/Hide to request the window-level dim backdrop.
/// </summary>
public interface IInkOverlayHost
{
    /// <summary>
    /// Register a control as an overlay consumer. Shows the dim backdrop if it was hidden.
    /// When the overlay is clicked, <paramref name="onDismiss"/> is invoked to close the control.
    /// </summary>
    void ShowOverlay(Visual registrant, Action onDismiss);

    /// <summary>
    /// Unregister a control. Hides the dim backdrop when no consumers remain.
    /// Safe to call even if the registrant was already removed (e.g. dismissed by overlay click).
    /// </summary>
    void HideOverlay(Visual registrant);
}
