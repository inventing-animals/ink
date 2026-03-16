# Data - Filtering, Sorting and Pagination

`Ink.Data` provides a platform-agnostic query model that is shared between UI components (DataGrid, Charts) and the server or data source that fulfils those queries. `Ink.Data.EFCore` translates that model directly into EF Core `IQueryable<T>` operations on the server.

## Installation

**Shared query model** - reference from both client and server projects:

```bash
dotnet add package InventingAnimals.Ink.Data
```

**EF Core server translator** - reference from your API host or server project only:

```bash
dotnet add package InventingAnimals.Ink.Data.EFCore
```

---

## Architecture

```
Ink.DataGrid / Ink.Charts  (UI layer, future)
        |
  Ink.Data            <- shared query model, no database or UI dependency
        |
  Ink.Data.EFCore     <- server side, translates queries to IQueryable<T>
  Ink.Data.Http       <- client side, sends queries over HTTP (future)
```

`Ink.Data` has no dependencies beyond the .NET base class library and `System.Text.Json`. It is safe to reference from WASM projects.

`Ink.Data.EFCore` depends on `Microsoft.EntityFrameworkCore` and is intended for server use only.

---

## DataGridQuery

`DataGridQuery` is the wire format sent from a DataGrid component to its data source.

```csharp
var query = new DataGridQuery(
    Columns: ["name", "email", "createdAt"],   // null means "all columns"
    Sort:    [new SortDescriptor("createdAt", SortDirection.Descending)],
    Filter:  new FilterCondition("active", FilterOp.Equal, [true]),
    Page:    1,
    PageSize: 25);
```

`DataGridQuery.Default` is a sensible starting point with no filter, no sort, page 1, page size 25:

```csharp
var query = DataGridQuery.Default with
{
    Sort = [new SortDescriptor("name", SortDirection.Ascending)],
};
```

---

## Filter trees

Filters are expressed as a composable tree of `FilterNode` values. Leaf nodes are `FilterCondition`; branch nodes are `FilterAnd`, `FilterOr`, and `FilterNot`.

```csharp
// Simple leaf
var filter = new FilterCondition("age", FilterOp.GreaterThan, [18]);

// Compound: active AND (age > 18 OR role = "Admin")
var filter = new FilterAnd([
    new FilterCondition("active", FilterOp.Equal, [true]),
    new FilterOr([
        new FilterCondition("age", FilterOp.GreaterThan, [18]),
        new FilterCondition("role", FilterOp.Equal, ["Admin"]),
    ]),
]);

// Negation
var filter = new FilterNot(new FilterCondition("archived", FilterOp.Equal, [true]));
```

### Combining conditions

When building filters from dynamic UI state the number of active conditions is not known upfront. The `.And()` and `.Or()` extension methods handle the empty and single-item cases for you:

```csharp
FilterNode? filter = conditions.And();
// empty list  -> null  (no filter applied)
// one item    -> that FilterCondition directly
// two or more -> FilterAnd wrapping the list
```

### Built-in operators

| `FilterOp` | Value | Notes |
|---|---|---|
| `FilterOp.Equal` | `"eq"` | |
| `FilterOp.NotEqual` | `"neq"` | |
| `FilterOp.Contains` | `"contains"` | string only |
| `FilterOp.StartsWith` | `"startsWith"` | string only |
| `FilterOp.EndsWith` | `"endsWith"` | string only |
| `FilterOp.LessThan` | `"lt"` | |
| `FilterOp.LessThanOrEqual` | `"lte"` | |
| `FilterOp.GreaterThan` | `"gt"` | |
| `FilterOp.GreaterThanOrEqual` | `"gte"` | |
| `FilterOp.In` | `"in"` | one or more values |
| `FilterOp.NotIn` | `"notIn"` | |
| `FilterOp.Between` | `"between"` | two values: min and max |
| `FilterOp.IsNull` | `"isNull"` | no values |
| `FilterOp.IsNotNull` | `"isNotNull"` | no values |

### Custom operators

`FilterOp` is a `readonly record struct` wrapping a string, so custom operators are just values:

```csharp
var fts = new FilterOp("fts");  // full-text search
var filter = new FilterCondition("description", fts, ["invoice"]);
```

Custom operators must be registered on the translator (see below) before the query is executed.

---

## JSON

`FilterNode` and `DataGridQuery` serialise to and from JSON out of the box using `System.Text.Json`. The `$type` discriminator identifies the node kind; `FilterOp` and `SortDirection` serialise as human-readable strings.

```json
{
  "Columns": ["name", "age"],
  "Sort": [{ "Field": "name", "Direction": "Ascending" }],
  "Filter": {
    "$type": "and",
    "Children": [
      { "$type": "condition", "Field": "active", "Op": "eq", "Values": [true] },
      { "$type": "condition", "Field": "age",    "Op": "gt", "Values": [18]   }
    ]
  },
  "Page": 1,
  "PageSize": 25
}
```

### Debug string

`FilterNodeExtensions.ToDebugString()` produces an indented human-readable representation useful for logging:

```csharp
Console.WriteLine(filter.ToDebugString());
// AND [
//   active eq true
//   age gt 18
// ]
```

---

## Examples

### Building queries from UI state

A typical scenario: a view model that maintains current filter and sort state and builds a `DataGridQuery` to fetch the next page.

```csharp
public class UserListViewModel
{
    private string _nameFilter = string.Empty;
    private bool? _activeFilter = null;
    private string _sortField = "name";
    private SortDirection _sortDirection = SortDirection.Ascending;
    private int _page = 1;

    public DataGridQuery BuildQuery()
    {
        var conditions = new List<FilterNode>();

        if (!string.IsNullOrWhiteSpace(_nameFilter))
            conditions.Add(new FilterCondition("name", FilterOp.Contains, [_nameFilter]));

        if (_activeFilter.HasValue)
            conditions.Add(new FilterCondition("active", FilterOp.Equal, [_activeFilter.Value]));

        return new DataGridQuery(
            Columns: null,
            Sort: [new SortDescriptor(_sortField, _sortDirection)],
            Filter: conditions.And(),
            Page: _page,
            PageSize: 25);
    }
}
```

### Common filter patterns

```csharp
// Exact match
new FilterCondition("status", FilterOp.Equal, ["Active"])

// Substring search
new FilterCondition("name", FilterOp.Contains, ["smith"])

// Date range
new FilterCondition("createdAt", FilterOp.Between, [
    new DateTime(2024, 1, 1),
    new DateTime(2024, 12, 31),
])

// Multiple allowed values
new FilterCondition("role", FilterOp.In, ["Admin", "Manager"])

// Null check
new FilterCondition("deletedAt", FilterOp.IsNull, [])

// Combined: active users created this year
new FilterAnd([
    new FilterCondition("active", FilterOp.Equal, [true]),
    new FilterCondition("createdAt", FilterOp.GreaterThanOrEqual, [new DateTime(2024, 1, 1)]),
])

// NOT archived, OR role is admin
new FilterOr([
    new FilterNot(new FilterCondition("archived", FilterOp.Equal, [true])),
    new FilterCondition("role", FilterOp.Equal, ["Admin"]),
])
```

### Multi-column sort

```csharp
var query = DataGridQuery.Default with
{
    Sort =
    [
        new SortDescriptor("department", SortDirection.Ascending),
        new SortDescriptor("name", SortDirection.Ascending),
    ],
};
```

### Column projection

When the server and client communicate over a narrow connection, request only the columns needed for the current view:

```csharp
var query = DataGridQuery.Default with
{
    Columns = ["name", "email", "createdAt"],
};
```

The translator enforces that requested columns are registered; unregistered column names throw `UnauthorizedFieldException`.

---

## EFCoreQueryTranslator

`EFCoreQueryTranslator<T>` is a configured, reusable object that applies a `DataGridQuery` to an `IQueryable<T>`. Configure it once as a singleton or static field.

### Setup

Configure the translator once - as a static field or a singleton registered in your DI container.

```csharp
// Registered as a singleton in Program.cs
builder.Services.AddSingleton(new EFCoreQueryTranslator<User>()
    .Column(x => x.Name)
    .Column(x => x.Email)
    .Column(x => x.Age)
    .Column(x => x.CreatedAt)
    .Column(x => x.Active));
```

Column field names are derived automatically from the expression in camelCase - `x => x.CreatedAt` registers the field `"createdAt"`. Matching is case-insensitive.

### Minimal API

The simplest wiring: a single `MapPost` endpoint that accepts a `DataGridQuery` and returns a `DataPage<T>`.

```csharp
app.MapPost("/api/users/query", async (
    DataGridQuery query,
    AppDbContext db,
    EFCoreQueryTranslator<User> translator) =>
{
    var result = await translator.ExecuteAsync(
        db.Users.Where(u => u.TenantId == currentTenantId),  // tenant isolation here
        query);
    return Results.Ok(result);
});
```

### ApiController

If you prefer a controller-based structure:

```csharp
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly EFCoreQueryTranslator<User> _translator;

    public UsersController(AppDbContext db, EFCoreQueryTranslator<User> translator)
    {
        _db = db;
        _translator = translator;
    }

    [HttpPost("query")]
    public async Task<ActionResult<DataPage<User>>> Query(
        [FromBody] DataGridQuery query,
        CancellationToken ct)
    {
        var baseQuery = _db.Users
            .Where(u => u.TenantId == User.GetTenantId());

        var result = await _translator.ExecuteAsync(baseQuery, query, ct);
        return Ok(result);
    }
}
```

`ExecuteAsync` runs two database queries - one for the total count, one for the page - and returns `DataPage<T>`:

```csharp
public record DataPage<T>(IReadOnlyList<T> Items, int TotalCount);
```

### Apply without pagination

Use `Apply` when you need the filtered and sorted `IQueryable<T>` but want to control pagination yourself or add further clauses:

```csharp
var q = translator.Apply(db.Users, query);
var count = await q.CountAsync(ct);
var items = await q
    .Skip(skip).Take(take)
    .Include(u => u.Department)
    .ToListAsync(ct);
```

### Security

Only columns explicitly registered via `.Column(...)` can be referenced in filters or sort descriptors. Any other field name throws `UnauthorizedFieldException`. This prevents clients from probing unintended fields such as password hashes.

```csharp
// UnauthorizedFieldException: 'passwordHash' is not registered
var query = DataGridQuery.Default with
{
    Filter = new FilterCondition("passwordHash", FilterOp.Equal, ["secret"]),
};
translator.Apply(db.Users, query);  // throws
```

Catch it in a global exception handler and return 400:

```csharp
app.UseExceptionHandler(err => err.Run(async ctx =>
{
    var ex = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
    if (ex is UnauthorizedFieldException ufe)
    {
        ctx.Response.StatusCode = 400;
        await ctx.Response.WriteAsJsonAsync(new { error = ufe.Message });
    }
}));
```

### Custom operators

Register a handler for any `FilterOp` value. The handler receives the member access expression, the member CLR type, and the condition values; it returns an EF Core-compatible predicate expression.

```csharp
// PostgreSQL full-text search via Npgsql
builder.Services.AddSingleton(new EFCoreQueryTranslator<Article>()
    .Column(x => x.Title)
    .Column(x => x.Body)
    .HandleOp(new FilterOp("fts"), (member, _, values) =>
        Expression.Call(
            typeof(NpgsqlDbFunctionsExtensions),
            nameof(NpgsqlDbFunctionsExtensions.ToTsQuery),
            null,
            Expression.Property(null, typeof(EF), nameof(EF.Functions)),
            member,
            Expression.Constant(values[0]?.ToString()))));
```

Custom handlers take precedence over built-in operators, so they can also override built-in behaviour for specific columns.

---

## Supported filter operations per column type

When `ops` is not specified, `EFCoreQueryTranslator<T>` infers the supported operations from the column value type:

| Column type | Default operators |
|---|---|
| `string` | `eq`, `neq`, `contains`, `startsWith`, `endsWith`, `in`, `notIn`, `isNull`, `isNotNull` |
| Numeric / `IComparable<T>` | `eq`, `neq`, `lt`, `lte`, `gt`, `gte`, `between`, `in`, `notIn` |
| `bool` | `eq`, `neq` |
| `enum` | `in`, `notIn` |

Override for a specific column by passing an explicit list:

```csharp
translator.Column(x => x.Status, ops: [FilterOp.In, FilterOp.NotIn]);
```
