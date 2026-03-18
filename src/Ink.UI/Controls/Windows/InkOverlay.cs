using System.Linq;
using Avalonia;
using Avalonia.VisualTree;

namespace Ink.UI.Controls;

/// <summary>
/// Shows or hides the window-level dim backdrop from any control.
/// Walks the visual tree upward to find the nearest IInkOverlayHost.
/// </summary>
public static class InkOverlay
{
    public static void Show(Visual from) =>
        FindHost(from)?.ShowOverlay();

    public static void Hide(Visual from) =>
        FindHost(from)?.HideOverlay();

    private static IInkOverlayHost? FindHost(Visual from) =>
        from.GetSelfAndVisualAncestors().OfType<IInkOverlayHost>().FirstOrDefault();
}
