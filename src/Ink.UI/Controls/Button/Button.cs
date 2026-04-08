using Avalonia;

namespace Ink.UI.Controls;

public class Button : Avalonia.Controls.Button
{
    public static readonly StyledProperty<ButtonVariant> VariantProperty =
        AvaloniaProperty.Register<Button, ButtonVariant>(nameof(Variant), ButtonVariant.Secondary);

    static Button()
    {
        VariantProperty.Changed.AddClassHandler<Button>(OnVariantChanged);
    }

    public Button()
    {
        ApplyVariantClass(Variant);
    }

    public ButtonVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    private static void OnVariantChanged(Button button, AvaloniaPropertyChangedEventArgs e)
    {
        button.ApplyVariantClass(e.GetNewValue<ButtonVariant>());
    }

    private void ApplyVariantClass(ButtonVariant variant)
    {
        ButtonVariantClassHelper.Apply(this, variant);
    }
}
