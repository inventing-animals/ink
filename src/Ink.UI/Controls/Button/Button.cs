using Avalonia;

namespace Ink.UI.Controls;

public class Button : Avalonia.Controls.Button
{
    public static readonly StyledProperty<ButtonVariant> VariantProperty =
        AvaloniaProperty.Register<Button, ButtonVariant>(nameof(Variant), ButtonVariant.Secondary);

    private static readonly string[] VariantClasses = ["ink-primary", "ink-secondary", "ink-ghost", "ink-danger", "ink-warning", "ink-success"];

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
