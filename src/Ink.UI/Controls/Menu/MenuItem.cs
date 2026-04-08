using Avalonia;

namespace Ink.UI.Controls;

public class MenuItem : Avalonia.Controls.MenuItem
{
    public static readonly StyledProperty<ButtonVariant> VariantProperty =
        AvaloniaProperty.Register<MenuItem, ButtonVariant>(nameof(Variant), ButtonVariant.Secondary);

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

    private static void OnVariantChanged(MenuItem menuItem, AvaloniaPropertyChangedEventArgs e)
    {
        menuItem.ApplyVariantClass(e.GetNewValue<ButtonVariant>());
    }

    private void ApplyVariantClass(ButtonVariant variant)
    {
        ButtonVariantClassHelper.Apply(this, variant);
    }
}
