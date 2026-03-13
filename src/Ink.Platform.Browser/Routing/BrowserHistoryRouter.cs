using System;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using Ink.Platform.Routing;

namespace Ink.Platform.Browser.Routing;

/// <summary>
/// A router backed by the browser's History API (<c>pushState</c> / <c>popstate</c>).
/// Suitable for WASM/browser platforms hosted on a server with URL rewriting enabled.
/// </summary>
/// <remarks>
/// If the app is served from a sub-path (e.g. <c>/myapp/</c>), add a
/// <c>&lt;base href="/myapp/"&gt;</c> element to <c>index.html</c>.
/// The router reads this at startup and automatically strips the base path from
/// <see cref="Current"/> and prepends it when pushing history entries, so the rest
/// of the app always works with root-relative paths like <c>/buttons</c>.
/// </remarks>
public partial class BrowserHistoryRouter : IRouter, IDisposable
{
    private static BrowserHistoryRouter? _instance;
    private readonly string _basePath;

    /// <summary>Initializes a new instance of the <see cref="BrowserHistoryRouter"/> class.</summary>
    /// <exception cref="InvalidOperationException">Thrown if an instance already exists.</exception>
    public BrowserHistoryRouter()
    {
        if (_instance is not null)
            throw new InvalidOperationException("Only one BrowserHistoryRouter may exist at a time.");

        _basePath = ResolveBasePath();
        _instance = this;
        RegisterPopState(() => _instance?.LocationChanged?.Invoke(_instance, _instance.Current));
    }

    /// <inheritdoc/>
    public event EventHandler<ILocation>? LocationChanged;

    /// <inheritdoc/>
    public ILocation Current
    {
        get
        {
            var location = Location.Parse(GetCurrentUrl());
            return _basePath.Length == 0 ? location : new Location(StripBase(location.Path), location.Query, location.Fragment);
        }
    }

    /// <inheritdoc/>
    public void Navigate(string path)
    {
        PushState(_basePath + path);
        LocationChanged?.Invoke(this, Current);
    }

    /// <inheritdoc/>
    public void Replace(string path)
    {
        ReplaceState(_basePath + path);
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

    private string StripBase(string path)
    {
        if (path.StartsWith(_basePath, StringComparison.OrdinalIgnoreCase))
            path = path[_basePath.Length..];

        return path.Length > 0 && path[0] == '/' ? path : "/" + path;
    }

    private static string ResolveBasePath()
    {
        var raw = GetBaseUrl();
        if (string.IsNullOrEmpty(raw)) return string.Empty;

        var absPath = new Uri(raw).AbsolutePath;
        return absPath == "/" ? string.Empty : absPath.TrimEnd('/');
    }

    [JSImport("globalThis.ink.router.getBaseUrl")]
    private static partial string GetBaseUrl();

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
