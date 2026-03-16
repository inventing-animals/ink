# DataGrid

`Ink.DataGrid` provides a DataGrid control for Avalonia applications. Columns and data are defined entirely in the view model - there is no XAML column configuration.

## Installation

```bash
dotnet add package InventingAnimals.Ink.DataGrid
```

Add the theme to your `App.axaml`:

```xml
<Application.Styles>
    <ink:InkTheme />
    <datagrid:DataGridTheme />
</Application.Styles>
```

With the namespaces:

```xml
xmlns:ink="using:Ink.UI.Themes"
xmlns:datagrid="using:Ink.DataGrid.Themes"
```

---

## Quick start

### 1. Define a view model

Inherit from `DataGridModel<T>` and set `Source` and `Columns` in the constructor.

```csharp
using Ink.Data.Columns;
using Ink.Data.Sources;
using Ink.DataGrid;

public class PeopleViewModel : DataGridModel<PersonRow>
{
    public PeopleViewModel()
    {
        Source = new ListDataSource<PersonRow>(
        [
            new("Alice Johnson",  "Engineering", "alice@example.com"),
            new("Bob Smith",      "Design",      "bob@example.com"),
            new("Carol Williams", "Engineering", "carol@example.com"),
        ]);

        Columns =
        [
            new Column<PersonRow, string>(x => x.Name)       { Header = "Name" },
            new Column<PersonRow, string>(x => x.Department) { Header = "Department" },
            new Column<PersonRow, string>(x => x.Email)      { Header = "Email" },
        ];
    }
}

public record PersonRow(string Name, string Department, string Email);
```

### 2. Add the control to your view

```xml
<UserControl xmlns:datagrid="clr-namespace:Ink.DataGrid.Controls;assembly=Ink.DataGrid"
             x:DataType="vm:PeopleViewModel">
    <datagrid:DataGrid Model="{Binding}" />
</UserControl>
```

That is all. The grid reads `Columns` and queries `Source` automatically.

---

## Data sources

`DataGridModel<T>` takes any `IDataSource<T>`. The interface has a single method:

```csharp
Task<DataPage<T>> QueryAsync(DataQuery query, CancellationToken ct = default);
```

### ListDataSource\<T\>

`Ink.Data` ships `ListDataSource<T>` for in-memory collections:

```csharp
Source = new ListDataSource<PersonRow>(items);
```

It applies `DataQuery.Page` and `DataQuery.PageSize` automatically. Useful for demos, tests, and small static datasets.

### Custom sources

Implement `IDataSource<T>` directly to connect any backend - an HTTP API, a database, a real-time stream.

```csharp
public class UsersHttpSource : IDataSource<UserRow>
{
    private readonly HttpClient _http;

    public UsersHttpSource(HttpClient http) => _http = http;

    public event Action? Invalidated { add { } remove { } }

    public async Task<DataPage<UserRow>> QueryAsync(DataQuery query, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("/api/users/query", query, ct);
        return await response.Content.ReadFromJsonAsync<DataPage<UserRow>>(ct)
               ?? new DataPage<UserRow>([], 0);
    }
}
```

Pass it to the view model:

```csharp
Source = new UsersHttpSource(httpClient);
```

---

## Columns

Columns are defined via `Column<TItem, TValue>` from `Ink.Data.Columns`. The selector expression is compiled once and cached.

```csharp
new Column<InvoiceRow, string>(x => x.Number)  { Header = "Number" }
new Column<InvoiceRow, decimal>(x => x.Total)  { Header = "Total" }
new Column<InvoiceRow, DateTime>(x => x.Date)  { Header = "Date" }
```

The `FieldName` is derived automatically from the expression in camelCase (`x => x.CreatedAt` gives `"createdAt"`), which matches the `DataQuery` field names used for sorting and filtering.

---

## Architecture

```
DataGridDemoViewModel : DataGridModel<PersonRow>
        |
  DataGridModel<T>          (Ink.DataGrid)
        |
  IDataSource<T>             (Ink.Data)
        |
  ListDataSource<T>          (Ink.Data)   <- or any custom IDataSource<T>
```

`Ink.Data` has no Avalonia dependency and can be referenced from server projects. `Ink.DataGrid` depends on both `Ink.Data` and `Avalonia`.
