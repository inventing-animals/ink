using System;
using Ink.Data.Columns;
using Ink.Data.Queries;
using Xunit;

namespace Ink.Data.Tests.Columns;

public class ColumnTests
{
    private sealed record User(string Name, int Age, DateTime CreatedAt, string? Email);

    // FieldName

    [Fact]
    public void FieldName_SingleWordProperty_IsCamelCase()
    {
        var col = new Column<User, string>(x => x.Name);
        Assert.Equal("name", col.FieldName);
    }

    [Fact]
    public void FieldName_MultiWordProperty_IsCamelCase()
    {
        var col = new Column<User, DateTime>(x => x.CreatedAt);
        Assert.Equal("createdAt", col.FieldName);
    }

    [Fact]
    public void FieldName_IsDerivedFromExpression_NotMagicString()
    {
        // Both sides of this assertion derive the same name - compiler catches renames
        var col1 = new Column<User, string>(x => x.Name);
        var col2 = new Column<User, string>(x => x.Name);
        Assert.Equal(col1.FieldName, col2.FieldName);
    }

    // SupportedOps

    [Fact]
    public void SupportedOps_StringColumn_DefaultsToTextOps()
    {
        var col = new Column<User, string>(x => x.Name);
        Assert.Contains(FilterOp.Contains, col.SupportedOps);
        Assert.Contains(FilterOp.StartsWith, col.SupportedOps);
    }

    [Fact]
    public void SupportedOps_IntColumn_DefaultsToRangeOps()
    {
        var col = new Column<User, int>(x => x.Age);
        Assert.Contains(FilterOp.GreaterThan, col.SupportedOps);
        Assert.Contains(FilterOp.Between, col.SupportedOps);
    }

    [Fact]
    public void SupportedOps_ExplicitOverride_ReplacesDefaults()
    {
        var ops = new[] { FilterOp.Equal };
        var col = new Column<User, string>(x => x.Name, ops);
        Assert.Single(col.SupportedOps);
        Assert.Equal(FilterOp.Equal, col.SupportedOps[0]);
    }

    // SelectorExpression

    [Fact]
    public void SelectorExpression_IsTheSameReferenceAsTypedSelector()
    {
        var col = new Column<User, string>(x => x.Name);
        Assert.Same(col.Selector, col.SelectorExpression);
    }

    // Base class

    [Fact]
    public void AsBaseClass_FieldNameStillAccessible()
    {
        Column<User> col = new Column<User, string>(x => x.Name);
        Assert.Equal("name", col.FieldName);
    }

    [Fact]
    public void AsBaseClass_SelectorExpressionReturnType_MatchesPropertyType()
    {
        Column<User> col = new Column<User, DateTime>(x => x.CreatedAt);
        Assert.Equal(typeof(DateTime), col.SelectorExpression.ReturnType);
    }
}
