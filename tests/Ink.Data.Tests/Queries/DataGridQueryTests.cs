using Ink.Data.Queries;
using Xunit;

namespace Ink.Data.Tests.Queries;

public class DataGridQueryTests
{
    [Fact]
    public void Default_IsPageOneWithTwentyFiveItems()
    {
        var q = DataGridQuery.Default;
        Assert.Equal(1, q.Page);
        Assert.Equal(25, q.PageSize);
    }

    [Fact]
    public void Default_HasNoFilterOrSort()
    {
        var q = DataGridQuery.Default;
        Assert.Null(q.Filter);
        Assert.Empty(q.Sort);
    }

    [Fact]
    public void Default_HasNullColumns_MeaningAll()
    {
        var q = DataGridQuery.Default;
        Assert.Null(q.Columns);
    }

    [Fact]
    public void With_CanOverridePageSize()
    {
        var q = DataGridQuery.Default with { PageSize = 50 };
        Assert.Equal(50, q.PageSize);
        Assert.Equal(1, q.Page);
    }

    [Fact]
    public void With_CanAttachFilter()
    {
        var filter = new FilterCondition("name", FilterOp.Contains, ["alice"]);
        var q = DataGridQuery.Default with { Filter = filter };
        Assert.NotNull(q.Filter);
        Assert.Equal(filter, q.Filter);
    }

    [Fact]
    public void With_CanAttachSort()
    {
        var sort = new SortDescriptor("createdAt", SortDirection.Descending);
        var q = DataGridQuery.Default with { Sort = [sort] };
        Assert.Single(q.Sort);
        Assert.Equal("createdAt", q.Sort[0].Field);
        Assert.Equal(SortDirection.Descending, q.Sort[0].Direction);
    }
}
