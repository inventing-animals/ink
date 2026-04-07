using Avalonia;
using Avalonia.Controls.Primitives;

namespace Ink.UI.Controls;

public class ComboBox : Avalonia.Controls.ComboBox
{
    private Popup? _popup;

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

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _popup = e.NameScope.Find("PART_Popup") as Popup;
        if (_popup is not null)
            _popup.OverlayDismissEventPassThrough = System.OperatingSystem.IsBrowser();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsDropDownOpenProperty)
        {
            if (change.GetNewValue<bool>())
                InkOverlay.Show(this, () => IsDropDownOpen = false);
            else
                InkOverlay.Hide(this);
        }
    }
}
