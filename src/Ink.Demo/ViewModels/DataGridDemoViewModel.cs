using Ink.Data.Columns;
using Ink.Data.Sources;
using Ink.DataGrid;

namespace Ink.Demo.ViewModels;

public class DataGridDemoViewModel : DataGridModel<PersonRow>
{
    public DataGridDemoViewModel()
    {
        Source = new ListDataSource<PersonRow>(
        [
            new("Alice Johnson",  "Engineering", "alice@example.com"),
            new("Bob Smith",      "Design",      "bob@example.com"),
            new("Carol Williams", "Engineering", "carol@example.com"),
            new("David Brown",    "Product",     "david@example.com"),
            new("Eva Garcia",     "Design",      "eva@example.com"),
            new("Frank Lee",      "Engineering", "frank@example.com"),
            new("Grace Kim",      "Marketing",   "grace@example.com"),
            new("Henry Chen",     "Product",     "henry@example.com"),
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
