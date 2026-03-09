using Avalonia;
using Avalonia.Controls;

namespace Ink.UI.Controls;

public static class ButtonAssist
{
    public static readonly AttachedProperty<ButtonVariant> VariantProperty =
        AvaloniaProperty.RegisterAttached<Button, ButtonVariant>(
            "Variant",
            typeof(ButtonAssist),
            ButtonVariant.Secondary);

    private static readonly string[] VariantClasses = ["ink-primary", "ink-secondary", "ink-ghost", "ink-danger"];

    static ButtonAssist()
    {
        VariantProperty.Changed.AddClassHandler<Button>(OnVariantChanged);
    }

    public static ButtonVariant GetVariant(Button button) =>
        button.GetValue(VariantProperty);

    public static void SetVariant(Button button, ButtonVariant value) =>
        button.SetValue(VariantProperty, value);

    private static void OnVariantChanged(Button button, AvaloniaPropertyChangedEventArgs e)
    {
        var variant = e.GetNewValue<ButtonVariant>();

        foreach (var cls in VariantClasses)
        {
            button.Classes.Set(cls, false);
        }

        var name = variant switch
        {
            ButtonVariant.Primary => "ink-primary",
            ButtonVariant.Secondary => "ink-secondary",
            ButtonVariant.Ghost => "ink-ghost",
            ButtonVariant.Danger => "ink-danger",
            _ => "ink-secondary",
        };

        button.Classes.Set(name, true);
    }
}
