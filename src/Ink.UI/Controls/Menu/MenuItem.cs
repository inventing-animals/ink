using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;

namespace Ink.UI.Controls;

public class MenuItem : Avalonia.Controls.MenuItem
{
    public static readonly StyledProperty<ButtonVariant> VariantProperty =
        AvaloniaProperty.Register<MenuItem, ButtonVariant>(nameof(Variant), ButtonVariant.Secondary);

    private Popup? _popup;
    private bool _overlayVisible;

    static MenuItem()
    {
        VariantProperty.Changed.AddClassHandler<MenuItem>(OnVariantChanged);
    }

    public MenuItem()
    {
        ApplyVariantClass(Variant);
    }

    public ButtonVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
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

        if (change.Property == IsSubMenuOpenProperty && IsTopLevelTrigger())
        {
            if (change.GetNewValue<bool>())
                ShowOverlay();
            else
                HideOverlay();
        }
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        HideOverlay();
        base.OnDetachedFromLogicalTree(e);
    }

    private static void OnVariantChanged(MenuItem menuItem, AvaloniaPropertyChangedEventArgs e)
    {
        menuItem.ApplyVariantClass(e.GetNewValue<ButtonVariant>());
    }

    private void ApplyVariantClass(ButtonVariant variant)
    {
        ButtonVariantClassHelper.Apply(this, variant);
    }

    private bool IsTopLevelTrigger() => Parent is Menu;

    private void ShowOverlay()
    {
        if (_overlayVisible)
            return;

        InkOverlay.Show(this, () => IsSubMenuOpen = false);
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
