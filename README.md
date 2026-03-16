# Ink

> [!WARNING]
> This project is in very early, heavy development. Expect breaking changes, missing features, and rough edges. It is not ready for use.

UI component library for [Avalonia](https://avaloniaui.net/) applications, supporting Desktop (Windows, Linux, macOS), Mobile (iOS, Android), and WASM.

**[Documentation](https://inventing-animals.github.io/ink/)** | **[Live demo](https://inventing-animals.github.io/ink/demo/)**

## What's included

### Ink

[![NuGet](https://img.shields.io/nuget/v/InventingAnimals.Ink)](https://www.nuget.org/packages/InventingAnimals.Ink)

`InventingAnimals.Ink` - UI components and theming for Avalonia applications. [Documentation](https://inventing-animals.github.io/ink/articles/intro.html)

- InkTheme with a built-in color palette, typography, and metric tokens
- Dark and light theme support out of the box

### Ink.Platform

[![NuGet](https://img.shields.io/nuget/v/InventingAnimals.Ink.Platform)](https://www.nuget.org/packages/InventingAnimals.Ink.Platform)

`InventingAnimals.Ink.Platform` - cross-platform service abstractions. [Documentation](https://inventing-animals.github.io/ink/articles/platform.html)

- `IWindowService` - opens a new window on Desktop/Mobile and a new browser tab on Web (always non-modal)
- `ISettingsService` / `FileSettingsService` - persistent key-value storage backed by a JSON file; trim and NativeAOT safe
- `IRouter` / `InMemoryRouter` - client-side navigation with full URL parsing (path, segments, query, fragment)

### Ink.Platform.Browser

[![NuGet](https://img.shields.io/nuget/v/InventingAnimals.Ink.Platform.Browser)](https://www.nuget.org/packages/InventingAnimals.Ink.Platform.Browser)

`InventingAnimals.Ink.Platform.Browser` - browser/WASM implementations of the platform services.

- `LocalStorageSettingsService` - settings backed by browser `localStorage`
- `BrowserHistoryRouter` - routing via the History API (`pushState` / `popstate`)

### Ink.Localization

[![NuGet](https://img.shields.io/nuget/v/InventingAnimals.Ink.Localization)](https://www.nuget.org/packages/InventingAnimals.Ink.Localization)

`InventingAnimals.Ink.Localization` - client and server localization with no Avalonia dependency.

- `ILocalizationService` / `LocalizationService` - client-side string lookup backed by `ResourceManager`
- `IApiLocalizationService` / `ApiLocalizationService` - server-side variant with explicit `CultureInfo` per call
- `Loc` - static ambient accessor for use in ViewModels (`Loc.Get("key")`, `Loc.Plural(...)`)
- CLDR plural rules via `PluralSelector`; trim and NativeAOT safe

### Ink.Data

[![NuGet](https://img.shields.io/nuget/v/InventingAnimals.Ink.Data)](https://www.nuget.org/packages/InventingAnimals.Ink.Data)

`InventingAnimals.Ink.Data` - shared query model for DataGrid and Charts with no Avalonia or database dependency. [Documentation](https://inventing-animals.github.io/ink/articles/data.html)

- `DataGridQuery` with composable filter trees (`FilterAnd`, `FilterOr`, `FilterNot`, `FilterCondition`)
- `FilterOp` - extensible operators; built-in set covers equality, comparison, range, null, in/not-in, and string ops
- `SortDescriptor`, `DataPage<T>`, `IDataGridSource<T>`, `IChartSource`
- Full JSON round-trip via `System.Text.Json`; WASM-safe

### Ink.Data.EFCore

[![NuGet](https://img.shields.io/nuget/v/InventingAnimals.Ink.Data.EFCore)](https://www.nuget.org/packages/InventingAnimals.Ink.Data.EFCore)

`InventingAnimals.Ink.Data.EFCore` - server-side EF Core query translator. [Documentation](https://inventing-animals.github.io/ink/articles/data.html)

- `EFCoreQueryTranslator<T>` - translates `DataGridQuery` into `IQueryable<T>` filter, sort, and pagination
- Type-safe column registration via expressions (`x => x.Name`); unregistered fields throw `UnauthorizedFieldException`
- Extensible with custom operators via `HandleOp`

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

For DataGrid and Charts data layer:

```
dotnet add package InventingAnimals.Ink.Data                 # client and server
dotnet add package InventingAnimals.Ink.Data.EFCore          # server only
```

## Contact

- Security isssues: security@inventing-animals.com
- Talk to us at: hello@inventing-animals.com

## License

MIT
