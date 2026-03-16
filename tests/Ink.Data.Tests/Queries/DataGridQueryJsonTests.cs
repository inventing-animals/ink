using System.Text.Json;
using Ink.Data.Queries;
using Xunit;

namespace Ink.Data.Tests.Queries;

public class DataGridQueryJsonTests
{
    [Fact]
    public void RoundTrip_DefaultQuery_Preserved()
    {
        var query = DataGridQuery.Default;
        var json = JsonSerializer.Serialize(query);
        var restored = JsonSerializer.Deserialize<DataGridQuery>(json)!;

        Assert.Equal(query.Page, restored.Page);
        Assert.Equal(query.PageSize, restored.PageSize);
        Assert.Null(restored.Filter);
        Assert.Empty(restored.Sort);
        Assert.Null(restored.Columns);
    }

    [Fact]
    public void RoundTrip_WithNestedFilter_PreservesTree()
    {
        var query = DataGridQuery.Default with
        {
            Filter = new FilterAnd([
                new FilterCondition("name", FilterOp.Contains, ["alice"]),
                new FilterOr([
                    new FilterCondition("role", FilterOp.Equal, ["Admin"]),
                    new FilterCondition("role", FilterOp.Equal, ["Editor"]),
                ]),
            ]),
        };

        var json = JsonSerializer.Serialize(query);
        var restored = JsonSerializer.Deserialize<DataGridQuery>(json)!;

        var and = Assert.IsType<FilterAnd>(restored.Filter);
        Assert.Equal(2, and.Children.Count);
        Assert.IsType<FilterOr>(and.Children[1]);
    }

    [Fact]
    public void RoundTrip_WithSort_PreservesSortDescriptors()
    {
        var query = DataGridQuery.Default with
        {
            Sort = [
                new SortDescriptor("createdAt", SortDirection.Descending),
                new SortDescriptor("name", SortDirection.Ascending),
            ],
        };

        var json = JsonSerializer.Serialize(query);
        var restored = JsonSerializer.Deserialize<DataGridQuery>(json)!;

        Assert.Equal(2, restored.Sort.Count);
        Assert.Equal("createdAt", restored.Sort[0].Field);
        Assert.Equal(SortDirection.Descending, restored.Sort[0].Direction);
        Assert.Equal("name", restored.Sort[1].Field);
    }

    [Fact]
    public void RoundTrip_WithColumns_PreservesColumnList()
    {
        var query = DataGridQuery.Default with { Columns = ["name", "email", "createdAt"] };

        var json = JsonSerializer.Serialize(query);
        var restored = JsonSerializer.Deserialize<DataGridQuery>(json)!;

        Assert.Equal(3, restored.Columns!.Count);
        Assert.Contains("email", restored.Columns);
    }

    [Fact]
    public void Serialize_IsHumanReadable()
    {
        var query = DataGridQuery.Default with
        {
            Filter = new FilterCondition("name", FilterOp.Contains, ["alice"]),
            Sort = [new SortDescriptor("createdAt", SortDirection.Descending)],
        };

        var json = JsonSerializer.Serialize(query, new JsonSerializerOptions { WriteIndented = true });

        // FilterOp as readable string, not a number
        Assert.Contains("\"contains\"", json);
        // FilterNode type as readable discriminator
        Assert.Contains("\"condition\"", json);
        // SortDirection as readable name
        Assert.Contains("Descending", json);
    }

    [Fact]
    public void Serialize_FullQuery_MatchesDocument()
    {
        var query = new DataGridQuery(
            Columns: ["name", "age"],
            Sort: [new SortDescriptor("name", SortDirection.Ascending)],
            Filter: new FilterCondition("active", FilterOp.Equal, [true]),
            Page: 2,
            PageSize: 10);

        var json = JsonSerializer.Serialize(query);

        Assert.Equal(
            "{\"Columns\":[\"name\",\"age\"]," +
            "\"Sort\":[{\"Field\":\"name\",\"Direction\":\"Ascending\"}]," +
            "\"Filter\":{\"$type\":\"condition\",\"Field\":\"active\",\"Op\":\"eq\",\"Values\":[true]}," +
            "\"Page\":2,\"PageSize\":10}",
            json);
    }

    [Fact]
    public void RoundTrip_WithCustomFilterOp_PreservesOp()
    {
        var fts = new FilterOp("fts");
        var query = DataGridQuery.Default with
        {
            Filter = new FilterCondition("description", fts, ["search term"]),
        };

        var json = JsonSerializer.Serialize(query);
        Assert.Contains("\"fts\"", json);

        var restored = JsonSerializer.Deserialize<DataGridQuery>(json)!;
        var cond = Assert.IsType<FilterCondition>(restored.Filter);
        Assert.Equal(fts, cond.Op);
    }
}
