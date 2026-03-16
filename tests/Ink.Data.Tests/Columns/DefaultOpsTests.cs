using System;
using Ink.Data.Columns;
using Ink.Data.Queries;
using Xunit;

namespace Ink.Data.Tests.Columns;

public class DefaultOpsTests
{
    private enum Status { Active, Inactive }

    // String

    [Fact]
    public void For_String_IncludesAllTextOps()
    {
        var ops = DefaultOps.For<string>();
        Assert.Contains(FilterOp.Contains, ops);
        Assert.Contains(FilterOp.StartsWith, ops);
        Assert.Contains(FilterOp.EndsWith, ops);
        Assert.Contains(FilterOp.Equal, ops);
        Assert.Contains(FilterOp.NotEqual, ops);
        Assert.Contains(FilterOp.IsNull, ops);
        Assert.Contains(FilterOp.IsNotNull, ops);
    }

    [Fact]
    public void For_String_ExcludesRangeOps()
    {
        var ops = DefaultOps.For<string>();
        Assert.DoesNotContain(FilterOp.GreaterThan, ops);
        Assert.DoesNotContain(FilterOp.Between, ops);
    }

    // Comparable (int, DateTime, etc.)

    [Fact]
    public void For_Int_IncludesRangeOps()
    {
        var ops = DefaultOps.For<int>();
        Assert.Contains(FilterOp.LessThan, ops);
        Assert.Contains(FilterOp.LessThanOrEqual, ops);
        Assert.Contains(FilterOp.GreaterThan, ops);
        Assert.Contains(FilterOp.GreaterThanOrEqual, ops);
        Assert.Contains(FilterOp.Between, ops);
    }

    [Fact]
    public void For_DateTime_IncludesRangeOps()
    {
        var ops = DefaultOps.For<DateTime>();
        Assert.Contains(FilterOp.GreaterThan, ops);
        Assert.Contains(FilterOp.Between, ops);
    }

    // Nullable stripping

    [Fact]
    public void For_NullableInt_SameAsNonNullable()
    {
        var nullableOps = DefaultOps.For<int?>();
        var nonNullableOps = DefaultOps.For<int>();
        Assert.Equal(nullableOps, nonNullableOps);
    }

    // Bool

    [Fact]
    public void For_Bool_OnlyEqualityOps()
    {
        var ops = DefaultOps.For<bool>();
        Assert.Contains(FilterOp.Equal, ops);
        Assert.Contains(FilterOp.NotEqual, ops);
        Assert.DoesNotContain(FilterOp.Contains, ops);
        Assert.DoesNotContain(FilterOp.GreaterThan, ops);
        Assert.DoesNotContain(FilterOp.Between, ops);
    }

    // Enum

    [Fact]
    public void For_Enum_IncludesSetOps()
    {
        var ops = DefaultOps.For<Status>();
        Assert.Contains(FilterOp.Equal, ops);
        Assert.Contains(FilterOp.NotEqual, ops);
        Assert.Contains(FilterOp.In, ops);
        Assert.Contains(FilterOp.NotIn, ops);
    }

    [Fact]
    public void For_Enum_ExcludesTextAndRangeOps()
    {
        var ops = DefaultOps.For<Status>();
        Assert.DoesNotContain(FilterOp.Contains, ops);
        Assert.DoesNotContain(FilterOp.GreaterThan, ops);
        Assert.DoesNotContain(FilterOp.Between, ops);
    }
}
