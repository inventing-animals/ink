using Avalonia;
using Avalonia.Controls.Primitives;

namespace Ink.UI.Controls;

public class ComboBox : Avalonia.Controls.ComboBox
{
    public static readonly StyledProperty<bool?> OverlayEnabledProperty =
        AvaloniaProperty.Register<ComboBox, bool?>(nameof(OverlayEnabled));

    private Popup? _popup;
    private bool _overlayVisible;

    /// <summary>
    /// Optional content placed at the top of the dropdown, above the items list.
    /// Defined as an attached property so it can be referenced in the base ComboBox ControlTheme.
    /// Intended for <see cref="PopupHeader"/> but accepts any content.
    /// </summary>
    public static readonly AttachedProperty<object?> PopupHeaderProperty =
        AvaloniaProperty.RegisterAttached<ComboBox, Avalonia.Controls.ComboBox, object?>("PopupHeader");

    public static object? GetPopupHeader(Avalonia.Controls.ComboBox control) =>
        control.GetValue(PopupHeaderProperty);

    public static void SetPopupHeader(Avalonia.Controls.ComboBox control, object? value) =>
        control.SetValue(PopupHeaderProperty, value);

    public object? PopupHeader
    {
        get => GetValue(PopupHeaderProperty);
        set => SetValue(PopupHeaderProperty, value);
    }

    public bool? OverlayEnabled
    {
        get => GetValue(OverlayEnabledProperty);
        set => SetValue(OverlayEnabledProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _popup = e.NameScope.Find("PART_Popup") as Popup;
        SyncPopupBehavior();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsDropDownOpenProperty
            || change.Property == OverlayEnabledProperty)
            SyncOverlayState();
    }

    protected override void OnDetachedFromLogicalTree(Avalonia.LogicalTree.LogicalTreeAttachmentEventArgs e)
    {
        HideOverlay();
        base.OnDetachedFromLogicalTree(e);
    }

    private void SyncOverlayState()
    {
        SyncPopupBehavior();

        if (IsDropDownOpen && IsOverlayEnabled())
            ShowOverlay();
        else
            HideOverlay();
    }

    private void SyncPopupBehavior()
    {
        if (_popup is not null)
            _popup.OverlayDismissEventPassThrough = System.OperatingSystem.IsBrowser() && IsOverlayEnabled();
    }

    private bool IsOverlayEnabled() => OverlayEnabled ?? true;

    private void ShowOverlay()
    {
        if (_overlayVisible)
            return;

        InkOverlay.Show(this, () => IsDropDownOpen = false);
        _overlayVisible = true;
    }

    private void HideOverlay()
    {
        if (!_overlayVisible)
            return;

        InkOverlay.Hide(this);
        _overlayVisible = false;
    }
}
