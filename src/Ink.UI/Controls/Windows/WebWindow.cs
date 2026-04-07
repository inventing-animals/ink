using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Ink.UI.Controls;

/// <summary>
/// Root view for browser/WASM applications built with Ink UI.
/// Use as the <see cref="Avalonia.Controls.UserControl"/> in your web entry point's single-view lifetime.
/// Set <see cref="InkBaseWindow.MainContent"/> to place the application view.
/// </summary>
public class WebWindow : InkBaseWindow
{
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (e.NameScope.Find("PART_VisualLayerManager") is not VisualLayerManager layerManager)
            return;

        // Avalonia's popup overlay host relies on an internal PopupOverlayLayer toggle.
        // Browser apps use WebWindow as a UserControl root rather than a TopLevel template,
        // so we enable the popup layer here to give overlay popups a place to render.
        var enablePopupOverlayLayer = typeof(VisualLayerManager).GetProperty(
            "EnablePopupOverlayLayer",
            BindingFlags.Instance | BindingFlags.NonPublic);
        enablePopupOverlayLayer?.SetValue(layerManager, true);

        var popupOverlayLayer = typeof(VisualLayerManager).GetProperty(
            "PopupOverlayLayer",
            BindingFlags.Instance | BindingFlags.NonPublic);
        _ = popupOverlayLayer?.GetValue(layerManager);
    }
}
