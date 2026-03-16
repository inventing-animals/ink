using Ink.Data.Queries;
using Xunit;

namespace Ink.Data.Tests.Queries;

public class FilterOpTests
{
    // Value semantics

    [Fact]
    public void SameValue_AreEqual()
    {
        var a = new FilterOp("contains");
        var b = new FilterOp("contains");
        Assert.Equal(a, b);
    }

    [Fact]
    public void DifferentValues_AreNotEqual()
    {
        Assert.NotEqual(FilterOp.Contains, FilterOp.Equal);
    }

    [Fact]
    public void StaticField_MatchesExplicitConstruction()
    {
        Assert.Equal(FilterOp.Contains, new FilterOp("contains"));
        Assert.Equal(FilterOp.GreaterThan, new FilterOp("gt"));
        Assert.Equal(FilterOp.Between, new FilterOp("between"));
    }

    // ToString

    [Fact]
    public void ToString_ReturnsValue()
    {
        Assert.Equal("contains", FilterOp.Contains.ToString());
        Assert.Equal("gt", FilterOp.GreaterThan.ToString());
    }

    // Custom ops

    [Fact]
    public void CustomOp_CanBeCreated()
    {
        var fts = new FilterOp("fts");
        Assert.Equal("fts", fts.Value);
        Assert.NotEqual(FilterOp.Contains, fts);
    }

    [Fact]
    public void CustomOp_CanBeUsedInFilterCondition()
    {
        var fts = new FilterOp("fts");
        var cond = new FilterCondition("description", fts, ["search term"]);
        Assert.Equal(fts, cond.Op);
    }
}
