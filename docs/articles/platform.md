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

| Platform | What opens | Content |
|----------|-----------|---------|
| Desktop | New `DesktopWindow` | `Func<Control>` factory |
| Mobile | Bottom-sheet drawer via `OverlayLayer` | `Func<Control>` factory |
| Web | New browser tab (`window.open`) | Fresh app instance at `WindowOptions.Url` |

### Design constraint: always non-modal

Windows are **always non-modal** by design. This is a deliberate choice driven by the web platform — a new browser tab is a completely separate WASM runtime with its own memory, so blocking the caller or sharing object references across tabs is impossible.

Rather than expose different behaviour per platform, `IWindowService` enforces the most constrained option everywhere:

- `OpenAsync` returns immediately with an `IWindowHandle`
- The app remains fully interactive while a secondary surface is open
- No return values, no shared state through the service itself

Cross-window communication must go through an external channel — a shared backend, `BroadcastChannel` on web, or an in-process event bus on desktop/mobile. A platform-uniform abstraction for this is planned.

### Usage

```csharp
var handle = await _windows.OpenAsync(
    () => new DetailView { DataContext = new DetailViewModel() },
    new WindowOptions
    {
        Title  = "Detail",
        Width  = 480,
        Height = 320,
        Url    = "/detail",   // used by the web platform
    });

// Optionally wait for the surface to be dismissed
await handle.WaitForCloseAsync();

// Or close it programmatically
handle.Close();
```

### Implementations

The appropriate implementation is wired up automatically in `App.axaml.cs`. To override the browser implementation, set `App.WindowServiceFactory` before the app starts:

```csharp
// Ink.Demo.Browser / Program.cs
App.WindowServiceFactory = () => new BrowserWindowService();
```

### `WindowOptions.Url` on web

On the web platform, `Url` is the router path the new tab navigates to. The new tab is a fully independent app instance — it will start at that path via `BrowserHistoryRouter`. If `Url` is not set, the tab opens at `/`.

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

router.LocationChanged += (_, location) =>
{
    Console.WriteLine(location.Path);
};

router.Navigate("/reports/123");
router.Navigate("/reports/123?sort=asc#summary");
router.Back();
router.Forward();
router.Replace("/reports/456");
```

### Browser setup

Register `BrowserHistoryRouter` in your DI container:

```csharp
services.AddSingleton<IRouter, BrowserHistoryRouter>();
```

Include the JS helper in your WASM host page to wire up `popstate`:

```html
<script type="module" src="_content/InventingAnimals.Ink.Platform.Browser/router.js"></script>
```

Your Azure App Service needs a URL rewrite rule to serve `index.html` for all paths so that deep links and browser refreshes work correctly.
