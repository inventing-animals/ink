# Platform Services

`Ink.Platform` provides cross-platform service abstractions for Ink UI applications. `Ink.Platform.Browser` provides WASM/browser implementations of those abstractions.

## Installation

```bash
dotnet add package InventingAnimals.Ink.Platform
```

For browser/WASM projects:

```bash
dotnet add package InventingAnimals.Ink.Platform.Browser
```

---

## Window Service

`IWindowService` opens secondary UI surfaces in a platform-appropriate way.

### Behaviour per platform

| Platform | `OpenAsync` | `OpenTabAsync` |
|----------|-------------|----------------|
| Desktop (`DesktopWindowService`) | New `DesktopWindow` | *(falls back to new window)* |
| Desktop tabbed (`DesktopTabbedWindowService`) | New `DesktopWindow` | Tab inside the existing `DesktopTabbedWindow` |
| Mobile (`DrawerWindowService`) | Bottom-sheet drawer | *(falls back to drawer)* |
| Web (`BrowserWindowService`) | New browser tab | *(falls back to new tab)* |

### Design constraint: always non-modal

Windows are **always non-modal** by design. This is a deliberate choice driven by the web platform — a new browser tab is a completely separate WASM runtime with its own memory, so blocking the caller or sharing object references across tabs is impossible.

Rather than expose different behaviour per platform, `IWindowService` enforces the most constrained option everywhere:

- `OpenAsync` and `OpenTabAsync` return immediately with an `IWindowHandle`
- The app remains fully interactive while a secondary surface is open
- No return values, no shared state through the service itself

Cross-window communication must go through an external channel — a shared backend, `BroadcastChannel` on web, or an in-process event bus on desktop/mobile.

### Usage

```csharp
// Open a new window (or platform equivalent)
var handle = await _windows.OpenAsync(
    () => new DetailView { DataContext = new DetailViewModel() },
    new WindowOptions
    {
        Title  = "Detail",
        Width  = 480,
        Height = 320,
        Url    = "/detail",   // used by the web platform
    });

// Open as a tab when the platform supports it; falls back to OpenAsync otherwise
var handle = await _windows.OpenTabAsync(
    () => new DetailView { DataContext = new DetailViewModel() },
    new WindowOptions { Title = "Detail", Url = "/detail" });

// Optionally wait for the surface to be dismissed
await handle.WaitForCloseAsync();

// Or close it programmatically
handle.Close();
```

### Desktop tabbed window

Use `DesktopTabbedWindow` as your main window and `DesktopTabbedWindowService` as `IWindowService` to have secondary surfaces open as tabs rather than new OS windows.

**`MainWindow.axaml.cs`**

```csharp
public partial class MainWindow : DesktopTabbedWindow
{
    public MainWindow() => InitializeComponent();
}
```

**`App.axaml.cs`** (desktop branch)

```csharp
var mainWindow = new MainWindow();
var appState   = new AppState(
    RouterFactory(),
    new DesktopTabbedWindowService(mainWindow),
    new ThemeService());

mainWindow.MainContent = new MainView { DataContext = new MainViewModel(appState) };
desktop.MainWindow = mainWindow;
```

When a tab is opened, the tab strip appears at the top of the window. The first entry in the strip is always the main window (non-closeable); subsequent entries are the tabs opened via `OpenTabAsync`. When all secondary tabs are closed the strip hides and the main content is restored.

### `WindowOptions.Url` on web

On the web platform, `Url` is the router path the new tab navigates to. The new tab is a fully independent app instance — it will start at that path via `BrowserHistoryRouter`. If `Url` is not set, the tab opens at `/`.

The URL is resolved against the app's base path automatically — see [Base URL / sub-path hosting](#base-url--sub-path-hosting) below.

---

## Settings

`ISettingsService` provides persistent key-value storage. All serialization uses `JsonTypeInfo<T>` for full trim and NativeAOT safety — no reflection at runtime.

### Implementations

| Class | Package | Backed by |
|---|---|---|
| `FileSettingsService` | `Ink.Platform` | JSON file on disk |
| `LocalStorageSettingsService` | `Ink.Platform.Browser` | Browser `localStorage` |

### Usage

Define a source-generated JSON context for your settings types:

```csharp
[JsonSerializable(typeof(UserPreferences))]
internal partial class AppJsonContext : JsonSerializerContext { }
```

Then use the service:

```csharp
// Desktop / mobile
ISettingsService settings = new FileSettingsService(
    Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "MyApp", "settings.json"));

// Read
var prefs = settings.Get("preferences", AppJsonContext.Default.UserPreferences);

// Write
settings.Set("preferences", new UserPreferences { Theme = "dark" }, AppJsonContext.Default.UserPreferences);

// Remove / check
settings.Remove("preferences");
bool exists = settings.Contains("preferences");
```

---

## Routing

`IRouter` manages client-side navigation state. `ILocation` gives you a fully parsed view of the current URL including path segments, query parameters, and fragment.

### Implementations

| Class | Package | Backed by |
|---|---|---|
| `InMemoryRouter` | `Ink.Platform` | In-memory history stack |
| `BrowserHistoryRouter` | `Ink.Platform.Browser` | Browser History API (`pushState` / `popstate`) |

`InMemoryRouter` is suitable for desktop, mobile, and unit tests. `BrowserHistoryRouter` is for WASM apps hosted on a server with URL rewriting enabled (e.g. Azure App Service).

### ILocation

Every navigation exposes an `ILocation`:

```csharp
// URL: /module/purchase-order/detail/123?sort=asc&page=2#notes

location.Path              // "/module/purchase-order/detail/123"
location.Segments          // ["module", "purchase-order", "detail", "123"]
location.Query             // "sort=asc&page=2"
location.QueryParameters   // { "sort": "asc", "page": "2" }
location.Fragment          // "notes"
```

### Usage

```csharp
IRouter router = new InMemoryRouter("/");

// Listen for navigation
router.LocationChanged += (_, location) =>
{
    Console.WriteLine(location.Path);
};

// Push a new history entry
router.Navigate("/reports/123");
router.Navigate("/reports/123?sort=asc#summary");

// History traversal
router.Back();
router.Forward();

// Replace current entry without adding to history
router.Replace("/reports/456");
```

### Route matching

Use `ILocation.Segments` to dispatch to pages or view models. The first segment is the top-level route; subsequent segments carry IDs or sub-routes.

```csharp
// Single-level routing
private ViewModelBase Resolve(ILocation location) =>
    location.Segments.FirstOrDefault() switch
    {
        "dashboard"  => new DashboardViewModel(),
        "reports"    => new ReportsViewModel(_router),
        "settings"   => new SettingsViewModel(),
        _            => new DashboardViewModel(),
    };

// Multi-segment routing — e.g. /reports/123/summary
private ViewModelBase ResolveReports(ILocation location) =>
    (location.Segments.ElementAtOrDefault(1),
     location.Segments.ElementAtOrDefault(2)) switch
    {
        (string id, "summary") => new ReportSummaryViewModel(id),
        (string id, _)         => new ReportDetailViewModel(id),
        _                      => new ReportListViewModel(),
    };

// Query parameters — e.g. /reports?page=2&sort=asc
var page = int.TryParse(location.QueryParameters.GetValueOrDefault("page"), out var p) ? p : 1;
var sort = location.QueryParameters.GetValueOrDefault("sort", "desc");

// Fragment — e.g. /reports/123#notes
var section = location.Fragment;  // "notes"
```

### Wiring the router to a view model

```csharp
public class MainViewModel
{
    private readonly IRouter _router;

    public MainViewModel(IRouter router)
    {
        _router = router;
        _router.LocationChanged += (_, location) => UpdatePage(location);
        UpdatePage(_router.Current);
    }

    private void UpdatePage(ILocation location)
    {
        CurrentPage = Resolve(location);
    }
}
```

### Browser setup

`BrowserHistoryRouter` requires a JavaScript helper object to be present at `globalThis.ink.router` before the WASM runtime starts. Add the following to your `main.js` (the module that boots the .NET runtime):

```javascript
globalThis.ink = {
    router: {
        getBaseUrl:       () => document.querySelector('base')?.href ?? '',
        getCurrentUrl:    () => window.location.href,
        pushState:        (path) => window.history.pushState(null, '', path),
        replaceState:     (path) => window.history.replaceState(null, '', path),
        back:             () => window.history.back(),
        forward:          () => window.history.forward(),
        registerPopState: (callback) => {
            window.addEventListener('popstate', () => callback());
        }
    }
};
```

Then set the factory before the app starts:

```csharp
// Program.cs
App.RouterFactory = () => new BrowserHistoryRouter();
```

### Base URL / sub-path hosting

When the app is served from a sub-path (e.g. `https://example.com/myapp/`), add a `<base>` element to `index.html`:

```html
<head>
    <base href="/myapp/" />
    ...
</head>
```

`BrowserHistoryRouter` reads `document.baseURI` at startup via `getBaseUrl` and automatically:

- **Strips** the base path from `Current` — the app always sees root-relative paths like `/reports/123`, never `/myapp/reports/123`
- **Prepends** the base path when pushing history entries via `Navigate` or `Replace`

`BrowserWindowService` also prepends the base path when opening new tabs, so `OpenAsync(..., new WindowOptions { Url = "/detail" })` correctly opens `https://example.com/myapp/detail`.

With no `<base>` tag (or `href="/"`), the router behaves as if hosted at root — nothing changes.

#### Setting BaseHref at publish time

For the Ink demo and similar CI deployments, the base href can be injected at publish time via an MSBuild property rather than hard-coded in `index.html`:

```bash
dotnet publish src/Ink.Demo.Browser -p:BaseHref=/ink-demo/
```

`index.html` ships with `<base href="/" />` as its default. The `SetBaseHref` MSBuild target rewrites the published copy when `BaseHref` differs from `/`. No manual file editing or server-side rendering is required.

See the deployment guides for platform-specific URL rewriting rules:
- [Azure App Service](deployment/azure-app-service.md)
- [GitHub Pages](deployment/github-pages.md)
- [nginx](deployment/nginx.md)
