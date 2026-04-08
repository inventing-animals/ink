using Avalonia.Controls;
using Avalonia;

namespace Ink.UI.Controls;

/// <summary>
/// Ink-styled flyout that integrates with the window overlay.
/// Shows the dim backdrop when opened and hides it when closed.
/// </summary>
public class Flyout : Avalonia.Controls.Flyout
{
    public static readonly StyledProperty<bool?> OverlayEnabledProperty =
        AvaloniaProperty.Register<Flyout, bool?>(nameof(OverlayEnabled));

    private bool _overlayVisible;

    public bool? OverlayEnabled
    {
        get => GetValue(OverlayEnabledProperty);
        set => SetValue(OverlayEnabledProperty, value);
    }

    protected override void OnOpened()
    {
        base.OnOpened();
        if (Target is not null)
        {
            var isBrowser = System.OperatingSystem.IsBrowser();
            var overlayEnabled = IsOverlayEnabled();

            // When the shared Ink overlay is disabled, fall back to normal popup
            // light-dismiss so clicking outside the flyout still closes it.
            Popup.IsLightDismissEnabled = isBrowser || !overlayEnabled;
            Popup.OverlayDismissEventPassThrough = isBrowser && overlayEnabled;

            if (overlayEnabled)
            {
                InkOverlay.Show(Target, Hide);
                _overlayVisible = true;
            }
        }
    }

    protected override void OnClosed()
    {
        base.OnClosed();
        Popup.IsLightDismissEnabled = true;
        Popup.OverlayDismissEventPassThrough = false;
        if (_overlayVisible && Target is not null)
        {
            InkOverlay.Hide(Target);
            _overlayVisible = false;
        }
    }

    private bool IsOverlayEnabled() => OverlayEnabled ?? true;
}
