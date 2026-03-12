using System;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace Ink.UI.Platform;

/// <summary>
/// Spawns secondary UI surfaces in a platform-appropriate way.
/// <list type="bullet">
///   <item>Desktop — new <see cref="Ink.UI.Controls.DesktopWindow"/></item>
///   <item>Mobile  — bottom-sheet drawer via <c>OverlayLayer</c></item>
///   <item>Web     — new browser tab loaded at <see cref="WindowOptions.Url"/></item>
/// </list>
/// Windows are always <b>non-modal</b>: the caller is never blocked, and the app
/// remains fully interactive while a secondary surface is open. This is a deliberate
/// constraint driven by the web platform, where true modal behaviour across tabs is
/// not possible.
/// </summary>
public interface IWindowService
{
    /// <param name="content">Factory that builds the content control (desktop and mobile only).</param>
    /// <param name="options">Optional title, size hints, and URL for the web platform.</param>
    /// <returns>A handle that lets the caller close the surface or await its dismissal.</returns>
    Task<IWindowHandle> OpenAsync(Func<Control> content, WindowOptions? options = null);
}
