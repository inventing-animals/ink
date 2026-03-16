using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Ink.UI.Controls;

namespace Ink.UI.Platform;

/// <summary>
/// Desktop implementation that combines window and tab support.
/// <list type="bullet">
///   <item><see cref="OpenAsync"/> - spawns a new OS window, identical to <see cref="DesktopWindowService"/>.</item>
///   <item><see cref="OpenTabAsync"/> - opens content as a tab inside the provided <see cref="DesktopTabbedWindow"/>.</item>
/// </list>
/// Pair this service with a <see cref="DesktopTabbedWindow"/> that the application owns
/// and shows as its main window. The window's lifecycle is managed by the caller.
/// </summary>
public sealed class DesktopTabbedWindowService : IWindowService
{
    private readonly DesktopTabbedWindow _window;

    /// <param name="window">The tab window that will host all tabbed content.</param>
    public DesktopTabbedWindowService(DesktopTabbedWindow window)
    {
        _window = window;
    }

    /// <inheritdoc />
    public Task<IWindowHandle> OpenAsync(Func<Control> content, WindowOptions? options = null)
    {
        var window = new DesktopWindow
        {
            Title = options?.Title ?? string.Empty,
            Content = content(),
        };

        if (options?.Width is { } w) window.Width = w;
        if (options?.Height is { } h) window.Height = h;

        var tcs = new TaskCompletionSource();
        window.Closed += (_, _) => tcs.TrySetResult();

        window.Show();

        return Task.FromResult<IWindowHandle>(new LambdaWindowHandle(window.Close, tcs.Task));
    }

    /// <inheritdoc />
    public Task<IWindowHandle> OpenTabAsync(Func<Control> content, WindowOptions? options = null)
        => Task.FromResult(_window.AddTab(options?.Title, content()));
}
