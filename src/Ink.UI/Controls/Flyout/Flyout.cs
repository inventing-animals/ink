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
            var isBrowser = System.OperatingSystem.IsBrowser();

            // Browser popups are hosted in Avalonia's overlay layer, so enable
            // light-dismiss there and let the click pass through to InkOverlayLayer.
            // Desktop keeps the existing native-popup behavior.
            Popup.IsLightDismissEnabled = isBrowser;
            Popup.OverlayDismissEventPassThrough = isBrowser;
            InkOverlay.Show(Target, Hide);
        }
    }

    protected override void OnClosed()
    {
        base.OnClosed();
        Popup.IsLightDismissEnabled = true;
        Popup.OverlayDismissEventPassThrough = false;
        if (Target is not null)
            InkOverlay.Hide(Target);
    }
}
