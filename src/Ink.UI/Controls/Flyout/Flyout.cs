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
            InkOverlay.Show(Target);
    }

    protected override void OnClosed()
    {
        base.OnClosed();
        if (Target is not null)
            InkOverlay.Hide(Target);
    }
}
