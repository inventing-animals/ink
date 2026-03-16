using Ink.Data.Queries;
using Xunit;

namespace Ink.Data.Tests.Queries;

public class DataQueryTests
{
    [Fact]
    public void Default_IsPageOneWithTwentyFiveItems()
    {
        var q = DataQuery.Default;
        Assert.Equal(1, q.Page);
        Assert.Equal(25, q.PageSize);
    }

    [Fact]
    public void Default_HasNoFilterOrSort()
    {
        var q = DataQuery.Default;
        Assert.Null(q.Filter);
        Assert.Empty(q.Sort);
    }

    [Fact]
    public void Default_HasNullColumns_MeaningAll()
    {
        var q = DataQuery.Default;
        Assert.Null(q.Columns);
    }

    [Fact]
    public void With_CanOverridePageSize()
    {
        var q = DataQuery.Default with { PageSize = 50 };
        Assert.Equal(50, q.PageSize);
        Assert.Equal(1, q.Page);
    }

    [Fact]
    public void With_CanAttachFilter()
    {
        var filter = new FilterCondition("name", FilterOp.Contains, ["alice"]);
        var q = DataQuery.Default with { Filter = filter };
        Assert.NotNull(q.Filter);
        Assert.Equal(filter, q.Filter);
    }

    [Fact]
    public void With_CanAttachSort()
    {
        var sort = new SortDescriptor("createdAt", SortDirection.Descending);
        var q = DataQuery.Default with { Sort = [sort] };
        Assert.Single(q.Sort);
        Assert.Equal("createdAt", q.Sort[0].Field);
        Assert.Equal(SortDirection.Descending, q.Sort[0].Direction);
    }
}
