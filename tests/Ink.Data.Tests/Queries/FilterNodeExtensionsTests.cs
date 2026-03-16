using System.Collections.Generic;
using Ink.Data.Queries;
using Xunit;

namespace Ink.Data.Tests.Queries;

public class FilterNodeExtensionsTests
{
    // Single condition

    [Fact]
    public void ToDebugString_SingleCondition_FormatsFieldOpValue()
    {
        var node = new FilterCondition("name", FilterOp.Contains, ["alice"]);
        var result = node.ToDebugString();
        Assert.Contains("name", result);
        Assert.Contains("contains", result);
        Assert.Contains("alice", result);
    }

    [Fact]
    public void ToDebugString_IsNullOp_EmptyValues()
    {
        var node = new FilterCondition("deletedAt", FilterOp.IsNull, []);
        var result = node.ToDebugString();
        Assert.Contains("deletedAt", result);
        Assert.Contains("isNull", result);
    }

    // Compound nodes

    [Fact]
    public void ToDebugString_And_ContainsAllChildren()
    {
        var node = new FilterAnd([
            new FilterCondition("name", FilterOp.Contains, ["alice"]),
            new FilterCondition("age", FilterOp.GreaterThan, [18]),
        ]);
        var result = node.ToDebugString();
        Assert.Contains("AND", result);
        Assert.Contains("name", result);
        Assert.Contains("age", result);
    }

    [Fact]
    public void ToDebugString_Or_ContainsAllChildren()
    {
        var node = new FilterOr([
            new FilterCondition("status", FilterOp.Equal, ["Active"]),
            new FilterCondition("status", FilterOp.Equal, ["Pending"]),
        ]);
        var result = node.ToDebugString();
        Assert.Contains("OR", result);
    }

    [Fact]
    public void ToDebugString_Not_ContainsChild()
    {
        var node = new FilterNot(new FilterCondition("archived", FilterOp.Equal, [true]));
        var result = node.ToDebugString();
        Assert.Contains("NOT", result);
        Assert.Contains("archived", result);
    }

    // Multiple values

    [Fact]
    public void ToDebugString_InOp_FormatsAsList()
    {
        var node = new FilterCondition("status", FilterOp.In, ["Active", "Pending"]);
        var result = node.ToDebugString();
        Assert.Contains("Active", result);
        Assert.Contains("Pending", result);
    }

    // Nesting produces indentation

    [Fact]
    public void ToDebugString_NestedTree_IsMultiline()
    {
        var node = new FilterAnd([
            new FilterCondition("name", FilterOp.Contains, ["alice"]),
            new FilterOr([
                new FilterCondition("role", FilterOp.Equal, ["Admin"]),
                new FilterCondition("role", FilterOp.Equal, ["Editor"]),
            ]),
        ]);
        var result = node.ToDebugString();
        Assert.Contains("\n", result);
        Assert.Contains("AND", result);
        Assert.Contains("OR", result);
    }

    // And / Or combinators

    [Fact]
    public void And_EmptyList_ReturnsNull()
    {
        var result = new List<FilterNode>().And();
        Assert.Null(result);
    }

    [Fact]
    public void And_SingleItem_ReturnsThatNode()
    {
        FilterNode cond = new FilterCondition("x", FilterOp.IsNull, []);
        var result = new List<FilterNode> { cond }.And();
        Assert.Same(cond, result);
    }

    [Fact]
    public void And_MultipleItems_ReturnsFilterAnd()
    {
        var nodes = new List<FilterNode>
        {
            new FilterCondition("a", FilterOp.IsNull, []),
            new FilterCondition("b", FilterOp.IsNull, []),
        };
        var result = Assert.IsType<FilterAnd>(nodes.And());
        Assert.Equal(2, result.Children.Count);
    }

    [Fact]
    public void Or_EmptyList_ReturnsNull()
    {
        Assert.Null(new List<FilterNode>().Or());
    }

    [Fact]
    public void Or_SingleItem_ReturnsThatNode()
    {
        FilterNode cond = new FilterCondition("x", FilterOp.IsNull, []);
        Assert.Same(cond, new List<FilterNode> { cond }.Or());
    }

    [Fact]
    public void Or_MultipleItems_ReturnsFilterOr()
    {
        var nodes = new List<FilterNode>
        {
            new FilterCondition("a", FilterOp.IsNull, []),
            new FilterCondition("b", FilterOp.IsNull, []),
        };
        var result = Assert.IsType<FilterOr>(nodes.Or());
        Assert.Equal(2, result.Children.Count);
    }
}
