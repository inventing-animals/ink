# Introduction

Ink UI is a component library for [Avalonia](https://avaloniaui.net/) applications.

## Installation

```bash
dotnet add package InventingAnimals.Ink
```

## Usage

Add the Ink theme to your `App.axaml`:

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="YourApp.App">
    <Application.Styles>
        <StyleInclude Source="avares://InventingAnimals.Ink/Themes/Ink.axaml" />
    </Application.Styles>
</Application>
```
