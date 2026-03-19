using Avalonia;

namespace Ink.UI.Controls;

public class Chip : Avalonia.Controls.ContentControl
{
    public static readonly StyledProperty<ChipVariant> VariantProperty =
        AvaloniaProperty.Register<Chip, ChipVariant>(nameof(Variant), ChipVariant.Flat);

    public static readonly StyledProperty<ChipRole> RoleProperty =
        AvaloniaProperty.Register<Chip, ChipRole>(nameof(Role), ChipRole.Secondary);

    private static readonly string[] VariantClasses = ["ink-chip-flat", "ink-chip-filled", "ink-chip-outline"];
    private static readonly string[] RoleClasses = ["ink-chip-primary", "ink-chip-secondary", "ink-chip-danger", "ink-chip-warning", "ink-chip-success"];

    static Chip()
    {
        VariantProperty.Changed.AddClassHandler<Chip>(OnVariantChanged);
        RoleProperty.Changed.AddClassHandler<Chip>(OnRoleChanged);
    }

    public Chip()
    {
        ApplyVariantClass(Variant);
        ApplyRoleClass(Role);
    }

    public ChipVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public ChipRole Role
    {
        get => GetValue(RoleProperty);
        set => SetValue(RoleProperty, value);
    }

    private static void OnVariantChanged(Chip chip, AvaloniaPropertyChangedEventArgs e)
    {
        chip.ApplyVariantClass(e.GetNewValue<ChipVariant>());
    }

    private static void OnRoleChanged(Chip chip, AvaloniaPropertyChangedEventArgs e)
    {
        chip.ApplyRoleClass(e.GetNewValue<ChipRole>());
    }

    private void ApplyVariantClass(ChipVariant variant)
    {
        foreach (var cls in VariantClasses)
            Classes.Set(cls, false);

        var name = variant switch
        {
            ChipVariant.Flat => "ink-chip-flat",
            ChipVariant.Filled => "ink-chip-filled",
            ChipVariant.Outline => "ink-chip-outline",
            _ => "ink-chip-flat",
        };

        Classes.Set(name, true);
    }

    private void ApplyRoleClass(ChipRole role)
    {
        foreach (var cls in RoleClasses)
            Classes.Set(cls, false);

        var name = role switch
        {
            ChipRole.Primary => "ink-chip-primary",
            ChipRole.Secondary => "ink-chip-secondary",
            ChipRole.Danger => "ink-chip-danger",
            ChipRole.Warning => "ink-chip-warning",
            ChipRole.Success => "ink-chip-success",
            _ => "ink-chip-secondary",
        };

        Classes.Set(name, true);
    }
}
