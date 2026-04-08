using Avalonia;

namespace Ink.UI.Controls;

public class ToggleButton : Avalonia.Controls.Primitives.ToggleButton
{
    public static readonly StyledProperty<ButtonVariant> VariantProperty =
        AvaloniaProperty.Register<ToggleButton, ButtonVariant>(nameof(Variant), ButtonVariant.Secondary);

    static ToggleButton()
    {
        VariantProperty.Changed.AddClassHandler<ToggleButton>(OnVariantChanged);
    }

    public ToggleButton()
    {
        ApplyVariantClass(Variant);
    }

    public ButtonVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    private static void OnVariantChanged(ToggleButton button, AvaloniaPropertyChangedEventArgs e)
    {
        button.ApplyVariantClass(e.GetNewValue<ButtonVariant>());
    }

    private void ApplyVariantClass(ButtonVariant variant)
    {
        ButtonVariantClassHelper.Apply(this, variant);
    }
}
