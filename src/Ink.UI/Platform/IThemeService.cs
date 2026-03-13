using System;
using Avalonia.Styling;

namespace Ink.UI.Platform;

/// <summary>
/// Controls the application-level theme variant (light / dark).
/// </summary>
public interface IThemeService
{
    /// <summary>Gets the currently active theme variant.</summary>
    ThemeVariant Current { get; }

    /// <summary>Raised whenever <see cref="Current"/> changes.</summary>
    event EventHandler<ThemeVariant>? ThemeChanged;

    /// <summary>Switches to the given theme variant.</summary>
    /// <param name="variant">The theme variant to apply.</param>
    void SetTheme(ThemeVariant variant);

    /// <summary>Toggles between <see cref="ThemeVariant.Light"/> and <see cref="ThemeVariant.Dark"/>.</summary>
    void Toggle();
}
