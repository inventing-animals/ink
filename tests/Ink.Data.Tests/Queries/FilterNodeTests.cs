using System;
using Ink.Data.Queries;
using Xunit;

namespace Ink.Data.Tests.Queries;

public class FilterNodeTests
{
    // FilterCondition

    [Fact]
    public void FilterCondition_StoresAllProperties()
    {
        var cond = new FilterCondition("name", FilterOp.Contains, ["john"]);
        Assert.Equal("name", cond.Field);
        Assert.Equal(FilterOp.Contains, cond.Op);
        Assert.Single(cond.Values);
        Assert.Equal("john", cond.Values[0]);
    }

    [Fact]
    public void FilterCondition_Between_HasTwoValues()
    {
        var cond = new FilterCondition("age", FilterOp.Between, [18, 65]);
        Assert.Equal(2, cond.Values.Count);
        Assert.Equal(18, cond.Values[0]);
        Assert.Equal(65, cond.Values[1]);
    }

    [Fact]
    public void FilterCondition_In_AcceptsMultipleValues()
    {
        var cond = new FilterCondition("status", FilterOp.In, ["Active", "Pending", "Review"]);
        Assert.Equal(3, cond.Values.Count);
    }

    [Fact]
    public void FilterCondition_IsNull_HasEmptyValues()
    {
        var cond = new FilterCondition("deletedAt", FilterOp.IsNull, []);
        Assert.Empty(cond.Values);
    }

    // FilterAnd

    [Fact]
    public void FilterAnd_StoresAllChildren()
    {
        var and = new FilterAnd([
            new FilterCondition("name", FilterOp.Contains, ["john"]),
            new FilterCondition("age", FilterOp.GreaterThan, [18]),
        ]);
        Assert.Equal(2, and.Children.Count);
    }

    [Fact]
    public void FilterAnd_IsAssignableToFilterNode()
    {
        FilterNode node = new FilterAnd([]);
        Assert.IsType<FilterAnd>(node);
    }

    // FilterOr

    [Fact]
    public void FilterOr_StoresAllChildren()
    {
        var or = new FilterOr([
            new FilterCondition("status", FilterOp.Equal, ["Active"]),
            new FilterCondition("status", FilterOp.Equal, ["Pending"]),
        ]);
        Assert.Equal(2, or.Children.Count);
    }

    // FilterNot

    [Fact]
    public void FilterNot_WrapsChild()
    {
        var cond = new FilterCondition("deleted", FilterOp.Equal, [true]);
        var not = new FilterNot(cond);
        Assert.Equal(cond, not.Child);
    }

    // Nesting

    [Fact]
    public void NestedTree_CanBeConstructed()
    {
        // (name CONTAINS "john") AND (status IN ["Active", "Pending"] OR age > 18)
        var tree = new FilterAnd([
            new FilterCondition("name", FilterOp.Contains, ["john"]),
            new FilterOr([
                new FilterCondition("status", FilterOp.In, ["Active", "Pending"]),
                new FilterCondition("age", FilterOp.GreaterThan, [18]),
            ]),
        ]);

        Assert.IsType<FilterAnd>(tree);
        var and = (FilterAnd)tree;
        Assert.Equal(2, and.Children.Count);
        Assert.IsType<FilterOr>(and.Children[1]);
    }

    [Fact]
    public void PatternMatch_WorksOnFilterNodeHierarchy()
    {
        FilterNode node = new FilterCondition("name", FilterOp.Equal, ["alice"]);

        var matched = node switch
        {
            FilterCondition c => c.Field,
            FilterAnd => "and",
            FilterOr => "or",
            FilterNot => "not",
            _ => throw new InvalidOperationException(),
        };

        Assert.Equal("name", matched);
    }
}
