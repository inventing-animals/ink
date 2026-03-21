using System;
using System.Linq;
using Avalonia;
using Avalonia.LogicalTree;

namespace Ink.UI.Controls;

/// <summary>
/// Shows or hides the window-level dim backdrop from any control.
/// Uses logical ancestors (not visual) so that controls inside popup content
/// — e.g. a ComboBox inside a Flyout — can still reach the window host.
/// Avalonia sets Popup.LogicalParent = placementTarget, bridging popup boundaries.
/// </summary>
public static class InkOverlay
{
    /// <summary>
    /// Register this control as an overlay consumer and show the dim backdrop.
    /// <paramref name="onDismiss"/> is invoked when the user clicks the overlay to close the control.
    /// </summary>
    public static void Show(Visual from, Action onDismiss) =>
        FindHost(from)?.ShowOverlay(from, onDismiss);

    /// <summary>Unregister this control from the overlay. Hides the backdrop when no consumers remain.</summary>
    public static void Hide(Visual from) =>
        FindHost(from)?.HideOverlay(from);

    private static IInkOverlayHost? FindHost(Visual from) =>
        ((ILogical)from).GetSelfAndLogicalAncestors().OfType<IInkOverlayHost>().FirstOrDefault();
}
