using Avalonia;

namespace Ink.UI.Controls;

internal static class ButtonVariantClassHelper
{
    private static readonly string[] VariantClasses = ["ink-primary", "ink-secondary", "ink-tertiary", "ink-ghost", "ink-danger", "ink-warning", "ink-success"];

    public static void Apply(StyledElement control, ButtonVariant variant)
    {
        foreach (var cls in VariantClasses)
            control.Classes.Set(cls, false);

        var name = variant switch
        {
            ButtonVariant.Primary => "ink-primary",
            ButtonVariant.Secondary => "ink-secondary",
            ButtonVariant.Tertiary => "ink-tertiary",
            ButtonVariant.Ghost => "ink-ghost",
            ButtonVariant.Danger => "ink-danger",
            ButtonVariant.Success => "ink-success",
            ButtonVariant.Warning => "ink-warning",
            _ => "ink-secondary",
        };

        control.Classes.Set(name, true);
    }
}
