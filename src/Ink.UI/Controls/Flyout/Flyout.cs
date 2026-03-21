using Avalonia.Controls;

namespace Ink.UI.Controls;

/// <summary>
/// Ink-styled flyout that integrates with the window overlay.
/// Shows the dim backdrop when opened and hides it when closed.
/// </summary>
public class Flyout : Avalonia.Controls.Flyout
{
    protected override void OnOpened()
    {
        base.OnOpened();
        if (Target is not null)
        {
            // Disable Avalonia's own light-dismiss so that clicking the overlay
            // is handled exclusively by InkOverlayLayer (LIFO stack). Without this,
            // Avalonia would close the flyout on any outside click regardless of
            // whether a nested popup (e.g. a ComboBox dropdown) should absorb it first.
            Popup.IsLightDismissEnabled = false;
            InkOverlay.Show(Target, Hide);
        }
    }

    protected override void OnClosed()
    {
        base.OnClosed();
        Popup.IsLightDismissEnabled = true;
        if (Target is not null)
            InkOverlay.Hide(Target);
    }
}
