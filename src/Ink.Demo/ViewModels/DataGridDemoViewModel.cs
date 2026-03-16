using System;
using Ink.Data.Sources;
using Ink.DataGrid;
using Ink.DataGrid.Columns;

namespace Ink.Demo.ViewModels;

public class DataGridDemoViewModel : DataGridModel<PersonRow>
{
    public DataGridDemoViewModel()
    {
        Source = new ListDataSource<PersonRow>(
        [
            new("Alice Johnson",  "Engineering", "alice@example.com",  31, true,  new DateTime(2019, 3, 15), Role.Engineer),
            new("Bob Smith",      "Design",      "bob@example.com",    28, true,  new DateTime(2021, 7,  1), Role.Designer),
            new("Carol Williams", "Engineering", "carol@example.com",  35, false, new DateTime(2017, 1, 20), Role.Engineer),
            new("David Brown",    "Product",     "david@example.com",  42, true,  new DateTime(2015, 9,  8), Role.Manager),
            new("Eva Garcia",     "Design",      "eva@example.com",    26, true,  new DateTime(2022, 4, 11), Role.Designer),
            new("Frank Lee",      "Engineering", "frank@example.com",  38, false, new DateTime(2018, 6, 30), Role.Analyst),
            new("Grace Kim",      "Marketing",   "grace@example.com",  33, true,  new DateTime(2020, 2, 14), Role.Analyst),
            new("Henry Chen",     "Product",     "henry@example.com",  45, true,  new DateTime(2013, 11, 5), Role.Manager),
        ]);

        Columns =
        [
            new DataGridColumn<PersonRow, string>  (x => x.Name)       { Header = "Name"       },
            new DataGridColumn<PersonRow, string>  (x => x.Department) { Header = "Department" },
            new DataGridColumn<PersonRow, string>  (x => x.Email)      { Header = "Email"      },
            new DataGridColumn<PersonRow, int>     (x => x.Age)        { Header = "Age"        },
            new DataGridColumn<PersonRow, bool>    (x => x.IsActive)   { Header = "Active"     },
            new DataGridColumn<PersonRow, DateTime>(x => x.HiredDate)  { Header = "Hired"      },
            new DataGridColumn<PersonRow, Role>    (x => x.Role)       { Header = "Role"       },
        ];
    }
}

public record PersonRow(
    string Name,
    string Department,
    string Email,
    int Age,
    bool IsActive,
    DateTime HiredDate,
    Role Role);

public enum Role { Engineer, Designer, Manager, Analyst }
