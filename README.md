# Ink

> [!WARNING]
> This project is in very early, heavy development. Expect breaking changes, missing features, and rough edges. It is not ready for use.

UI component library for [Avalonia](https://avaloniaui.net/) applications.

**[Documentation](https://inventing-animals.github.io/ink/)** | **[Live demo](https://inventing-animals.github.io/ink/demo/)**

## Packages

| Package | Description |
|---|---|
| `InventingAnimals.Ink` | UI components and themes |
| `InventingAnimals.Ink.Platform` | Cross-platform service abstractions (settings, routing) |
| `InventingAnimals.Ink.Platform.Browser` | Browser/WASM implementations (localStorage, History API) |

## Installation

```
dotnet add package InventingAnimals.Ink
```

For platform services:

```
dotnet add package InventingAnimals.Ink.Platform
dotnet add package InventingAnimals.Ink.Platform.Browser  # WASM projects only
```

## License

MIT
