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
