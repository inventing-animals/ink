using Avalonia;

namespace Ink.UI.Controls;

public class Chip : Avalonia.Controls.ContentControl
{
    public static readonly StyledProperty<ChipVariant> VariantProperty =
        AvaloniaProperty.Register<Chip, ChipVariant>(nameof(Variant), ChipVariant.Flat);

    public static readonly StyledProperty<ChipRole> RoleProperty =
        AvaloniaProperty.Register<Chip, ChipRole>(nameof(Role), ChipRole.Secondary);

    // Structural variant classes (used for bg shape / outline border visibility)
    private static readonly string[] VariantClasses = ["ink-chip-flat", "ink-chip-filled", "ink-chip-outline"];

    // Combined variant+role classes (used for color overrides; avoids compound selectors)
    private static readonly string[] CombinedClasses =
    [
        "ink-chip-flat-secondary",  "ink-chip-flat-primary",  "ink-chip-flat-danger",  "ink-chip-flat-warning",  "ink-chip-flat-success",
        "ink-chip-filled-secondary","ink-chip-filled-primary","ink-chip-filled-danger","ink-chip-filled-warning","ink-chip-filled-success",
        "ink-chip-outline-secondary","ink-chip-outline-primary","ink-chip-outline-danger","ink-chip-outline-warning","ink-chip-outline-success",
    ];

    static Chip()
    {
        VariantProperty.Changed.AddClassHandler<Chip>(OnVariantChanged);
        RoleProperty.Changed.AddClassHandler<Chip>(OnRoleChanged);
    }

    public Chip()
    {
        ApplyClasses(Variant, Role);
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
        => chip.ApplyClasses(e.GetNewValue<ChipVariant>(), chip.Role);

    private static void OnRoleChanged(Chip chip, AvaloniaPropertyChangedEventArgs e)
        => chip.ApplyClasses(chip.Variant, e.GetNewValue<ChipRole>());

    private void ApplyClasses(ChipVariant variant, ChipRole role)
    {
        foreach (var cls in VariantClasses) Classes.Set(cls, false);
        foreach (var cls in CombinedClasses) Classes.Set(cls, false);

        var variantPart = variant switch
        {
            ChipVariant.Flat    => "flat",
            ChipVariant.Filled  => "filled",
            ChipVariant.Outline => "outline",
            _                   => "flat",
        };

        var rolePart = role switch
        {
            ChipRole.Primary   => "primary",
            ChipRole.Secondary => "secondary",
            ChipRole.Danger    => "danger",
            ChipRole.Warning   => "warning",
            ChipRole.Success   => "success",
            _                  => "secondary",
        };

        Classes.Set($"ink-chip-{variantPart}", true);
        Classes.Set($"ink-chip-{variantPart}-{rolePart}", true);
    }
}
