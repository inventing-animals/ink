using Avalonia;

namespace Ink.UI.Controls;

public class ToggleButton : Avalonia.Controls.Primitives.ToggleButton
{
    public static readonly StyledProperty<ButtonVariant> VariantProperty =
        AvaloniaProperty.Register<ToggleButton, ButtonVariant>(nameof(Variant), ButtonVariant.Secondary);

    private static readonly string[] VariantClasses = ["ink-primary", "ink-secondary", "ink-ghost", "ink-danger", "ink-warning", "ink-success"];

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
        foreach (var cls in VariantClasses)
            Classes.Set(cls, false);

        var name = variant switch
        {
            ButtonVariant.Primary => "ink-primary",
            ButtonVariant.Secondary => "ink-secondary",
            ButtonVariant.Ghost => "ink-ghost",
            ButtonVariant.Danger => "ink-danger",
            ButtonVariant.Success => "ink-success",
            ButtonVariant.Warning => "ink-warning",
            _ => "ink-secondary",
        };

        Classes.Set(name, true);
    }
}
