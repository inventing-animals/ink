# Ink

> [!WARNING]
> This project is in very early, heavy development. Expect breaking changes, missing features, and rough edges. It is not ready for use.

UI component library for [Avalonia](https://avaloniaui.net/) applications.

**[Documentation](https://inventing-animals.github.io/ink/)** | **[Live demo](https://inventing-animals.github.io/ink/demo/)**

## What's included

**`InventingAnimals.Ink`** — components and theming
- InkTheme with a built-in color palette, typography, and metric tokens
- Dark and light theme support out of the box

**`InventingAnimals.Ink.Platform`** — cross-platform services
- `IWindowService` — opens a new window on Desktop/Mobile and a new browser tab on Web (always non-modal)
- `ISettingsService` / `FileSettingsService` — persistent key-value storage backed by a JSON file; trim and NativeAOT safe
- `IRouter` / `InMemoryRouter` — client-side navigation with full URL parsing (path, segments, query, fragment)

**`InventingAnimals.Ink.Platform.Browser`** — WASM implementations
- `LocalStorageSettingsService` — settings backed by browser `localStorage`
- `BrowserHistoryRouter` — routing via the History API (`pushState` / `popstate`)

**`InventingAnimals.Ink.Localization`** — localization (no Avalonia dependency)
- `ILocalizationService` / `LocalizationService` — client-side string lookup backed by `ResourceManager`
- `IApiLocalizationService` / `ApiLocalizationService` — server-side variant with explicit `CultureInfo` per call
- `Loc` — static ambient accessor for use in ViewModels (`Loc.Get("key")`, `Loc.Plural(...)`)
- CLDR plural rules via `PluralSelector`; trim and NativeAOT safe

## Packages

| Package | Version | Description |
|---|---|---|
| [`InventingAnimals.Ink`](https://www.nuget.org/packages/InventingAnimals.Ink) | [![NuGet](https://img.shields.io/nuget/v/InventingAnimals.Ink)](https://www.nuget.org/packages/InventingAnimals.Ink) | UI components and themes |
| [`InventingAnimals.Ink.Platform`](https://www.nuget.org/packages/InventingAnimals.Ink.Platform) | [![NuGet](https://img.shields.io/nuget/v/InventingAnimals.Ink.Platform)](https://www.nuget.org/packages/InventingAnimals.Ink.Platform) | Cross-platform service abstractions |
| [`InventingAnimals.Ink.Platform.Browser`](https://www.nuget.org/packages/InventingAnimals.Ink.Platform.Browser) | [![NuGet](https://img.shields.io/nuget/v/InventingAnimals.Ink.Platform.Browser)](https://www.nuget.org/packages/InventingAnimals.Ink.Platform.Browser) | Browser/WASM implementations |
| [`InventingAnimals.Ink.Localization`](https://www.nuget.org/packages/InventingAnimals.Ink.Localization) | [![NuGet](https://img.shields.io/nuget/v/InventingAnimals.Ink.Localization)](https://www.nuget.org/packages/InventingAnimals.Ink.Localization) | Localization abstractions and implementations |

## Installation

```
dotnet add package InventingAnimals.Ink
```

For platform services:

```
dotnet add package InventingAnimals.Ink.Platform
dotnet add package InventingAnimals.Ink.Platform.Browser  # WASM projects only
dotnet add package InventingAnimals.Ink.Localization
```

## Contact

- Security isssues: security@inventing-animals.com
- Talk to us at: hello@inventing-animals.com

## License

MIT
