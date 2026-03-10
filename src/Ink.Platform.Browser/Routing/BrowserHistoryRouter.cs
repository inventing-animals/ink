using System;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using Ink.Platform.Routing;

namespace Ink.Platform.Browser.Routing;

/// <summary>
/// A router backed by the browser's History API (<c>pushState</c> / <c>popstate</c>).
/// Suitable for WASM/browser platforms hosted on a server with URL rewriting enabled.
/// </summary>
public partial class BrowserHistoryRouter : IRouter, IDisposable
{
    private static BrowserHistoryRouter? _instance;

    /// <summary>Initializes a new instance of the <see cref="BrowserHistoryRouter"/> class.</summary>
    /// <exception cref="InvalidOperationException">Thrown if an instance already exists.</exception>
    public BrowserHistoryRouter()
    {
        if (_instance is not null)
        {
            throw new InvalidOperationException("Only one BrowserHistoryRouter may exist at a time.");
        }

        _instance = this;
        RegisterPopState(() => _instance?.LocationChanged?.Invoke(_instance, _instance.Current));
    }

    /// <inheritdoc/>
    public event EventHandler<ILocation>? LocationChanged;

    /// <inheritdoc/>
    public ILocation Current => Location.Parse(GetCurrentUrl());

    /// <inheritdoc/>
    public void Navigate(string path)
    {
        PushState(path);
        LocationChanged?.Invoke(this, Current);
    }

    /// <inheritdoc/>
    public void Replace(string path)
    {
        ReplaceState(path);
        LocationChanged?.Invoke(this, Current);
    }

    /// <inheritdoc/>
    public void Back() => HistoryBack();

    /// <inheritdoc/>
    public void Forward() => HistoryForward();

    /// <inheritdoc/>
    public void Dispose()
    {
        _instance = null;
    }

    [JSImport("globalThis.ink.router.getCurrentUrl")]
    private static partial string GetCurrentUrl();

    [JSImport("globalThis.ink.router.pushState")]
    private static partial void PushState(string path);

    [JSImport("globalThis.ink.router.replaceState")]
    private static partial void ReplaceState(string path);

    [JSImport("globalThis.ink.router.back")]
    private static partial void HistoryBack();

    [JSImport("globalThis.ink.router.forward")]
    private static partial void HistoryForward();

    [JSImport("globalThis.ink.router.registerPopState")]
    private static partial void RegisterPopState([JSMarshalAs<JSType.Function>] Action callback);
}
