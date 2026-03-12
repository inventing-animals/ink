using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Ink.UI.Controls;

namespace Ink.UI.Platform;

/// <summary>
/// Desktop implementation: opens content in a new <see cref="DesktopWindow"/>.
/// </summary>
public sealed class DesktopWindowService : IWindowService
{
    public Task<IWindowHandle> OpenAsync(Func<Control> content, WindowOptions? options = null)
    {
        var window = new DesktopWindow
        {
            Title   = options?.Title ?? string.Empty,
            Content = content(),
        };

        if (options?.Width  is { } w) window.Width  = w;
        if (options?.Height is { } h) window.Height = h;

        var tcs = new TaskCompletionSource();
        window.Closed += (_, _) => tcs.TrySetResult();

        window.Show();

        return Task.FromResult<IWindowHandle>(new LambdaWindowHandle(window.Close, tcs.Task));
    }
}
