using System;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace Ink.UI.Platform;

/// <summary>
/// Spawns secondary UI surfaces in a platform-appropriate way.
/// <list type="bullet">
///   <item>Desktop - new <see cref="Ink.UI.Controls.DesktopWindow"/></item>
///   <item>Desktop (tabbed) - tab inside the existing <see cref="Ink.UI.Controls.DesktopTabbedWindow"/></item>
///   <item>Mobile  - bottom-sheet drawer via <c>OverlayLayer</c></item>
///   <item>Web     - new browser tab loaded at <see cref="WindowOptions.Url"/></item>
/// </list>
/// Surfaces are always <b>non-modal</b>: the caller is never blocked, and the app
/// remains fully interactive while a secondary surface is open. This is a deliberate
/// constraint driven by the web platform, where true modal behaviour across tabs is
/// not possible.
/// </summary>
public interface IWindowService
{
    /// <summary>
    /// Opens content in a new window (or the platform equivalent).
    /// </summary>
    /// <param name="content">Factory that builds the content control (desktop and mobile only).</param>
    /// <param name="options">Optional title, size hints, and URL for the web platform.</param>
    /// <returns>A handle that lets the caller close the surface or await its dismissal.</returns>
    Task<IWindowHandle> OpenAsync(Func<Control> content, WindowOptions? options = null);

    /// <summary>
    /// Opens content as a tab when the platform supports it; falls back to
    /// <see cref="OpenAsync"/> otherwise.
    /// </summary>
    /// <param name="content">Factory that builds the content control (desktop and mobile only).</param>
    /// <param name="options">Optional title, size hints, and URL for the web platform.</param>
    /// <returns>A handle that lets the caller close the surface or await its dismissal.</returns>
    Task<IWindowHandle> OpenTabAsync(Func<Control> content, WindowOptions? options = null)
        => OpenAsync(content, options);
}
