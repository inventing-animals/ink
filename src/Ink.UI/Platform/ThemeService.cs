using System;
using Avalonia;
using Avalonia.Styling;

namespace Ink.UI.Platform;

/// <summary>
/// Default implementation: reads and writes <see cref="Application.RequestedThemeVariant"/>
/// on the current Avalonia application instance.
/// </summary>
public sealed class ThemeService : IThemeService
{
    /// <inheritdoc/>
    public ThemeVariant Current => Application.Current?.ActualThemeVariant ?? ThemeVariant.Light;

    /// <inheritdoc/>
    public event EventHandler<ThemeVariant>? ThemeChanged;

    /// <inheritdoc/>
    public void SetTheme(ThemeVariant variant)
    {
        if (Application.Current is null) return;
        Application.Current.RequestedThemeVariant = variant;
        ThemeChanged?.Invoke(this, variant);
    }

    /// <inheritdoc/>
    public void Toggle() =>
        SetTheme(Current == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark);
}
