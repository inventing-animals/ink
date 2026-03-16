using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Avalonia.Controls;
using Ink.UI.Platform;

namespace Ink.Demo.Browser;

/// <summary>
/// Browser implementation of <see cref="IWindowService"/>.
/// Opens a new tab via <c>window.open(url, "_blank")</c> - the tab loads a fresh
/// app instance which navigates to <see cref="WindowOptions.Url"/>.
/// The <c>Func&lt;Control&gt;</c> content factory is ignored; content is determined
/// entirely by the URL and the app's router.
/// </summary>
internal sealed partial class BrowserWindowService : IWindowService
{
    public Task<IWindowHandle> OpenAsync(Func<Control> content, WindowOptions? options = null)
    {
        Open(ResolveUrl(options?.Url ?? "/"), "_blank");

        // A browser tab is a separate process - we cannot track its lifecycle.
        // WaitForCloseAsync completes immediately; Close() is a no-op.
        return Task.FromResult<IWindowHandle>(new ImmediateHandle());
    }

    private static string ResolveUrl(string appPath)
    {
        var raw = GetBaseUrl();
        if (string.IsNullOrEmpty(raw)) return appPath;

        var absPath = new Uri(raw).AbsolutePath;
        var basePath = absPath == "/" ? string.Empty : absPath.TrimEnd('/');
        return basePath + appPath;
    }

    [JSImport("globalThis.ink.router.getBaseUrl")]
    private static partial string GetBaseUrl();

    [JSImport("globalThis.open")]
    private static partial void Open(string url, string target);

    private sealed class ImmediateHandle : IWindowHandle
    {
        public void Close() { }
        public Task WaitForCloseAsync() => Task.CompletedTask;
    }
}
