using System;
using System.Linq.Expressions;
using Ink.Data.Columns;
using Xunit;

namespace Ink.Data.Tests.Columns;

public class ExpressionHelperTests
{
    private sealed class Item
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public DateTime CreatedAt { get; set; }
        public string EmailAddress { get; set; } = string.Empty;
    }

    // GetMemberName

    [Fact]
    public void GetMemberName_SimpleProperty_ReturnsPropertyName()
    {
        Expression<Func<Item, string>> expr = x => x.Name;
        Assert.Equal("Name", ExpressionHelper.GetMemberName(expr));
    }

    [Fact]
    public void GetMemberName_BoxedValueProperty_ReturnsPropertyName()
    {
        // int boxing creates a UnaryExpression wrapping the MemberExpression
        Expression<Func<Item, object>> expr = x => x.Age;
        Assert.Equal("Age", ExpressionHelper.GetMemberName(expr));
    }

    [Fact]
    public void GetMemberName_MultiWordProperty_ReturnsFullName()
    {
        Expression<Func<Item, string>> expr = x => x.EmailAddress;
        Assert.Equal("EmailAddress", ExpressionHelper.GetMemberName(expr));
    }

    [Fact]
    public void GetMemberName_NonMemberExpression_ThrowsArgumentException()
    {
        Expression<Func<Item, string>> expr = x => x.Name + "suffix";
        Assert.Throws<ArgumentException>(() => ExpressionHelper.GetMemberName(expr));
    }

    // ToCamelCase

    [Fact]
    public void ToCamelCase_UppercaseFirstLetter_LowercasesIt()
    {
        Expression<Func<Item, string>> expr = x => x.Name;
        Assert.Equal("name", ExpressionHelper.ToCamelCase(expr));
    }

    [Fact]
    public void ToCamelCase_MultiWordProperty_OnlyFirstLetterLowercased()
    {
        Expression<Func<Item, DateTime>> expr = x => x.CreatedAt;
        Assert.Equal("createdAt", ExpressionHelper.ToCamelCase(expr));
    }

    [Fact]
    public void ToCamelCase_BoxedProperty_Works()
    {
        Expression<Func<Item, object>> expr = x => x.Age;
        Assert.Equal("age", ExpressionHelper.ToCamelCase(expr));
    }

    // GetMemberInfo

    [Fact]
    public void GetMemberInfo_SimpleProperty_ReturnsMemberInfo()
    {
        Expression<Func<Item, string>> expr = x => x.Name;
        var info = ExpressionHelper.GetMemberInfo(expr);
        Assert.Equal("Name", info.Name);
    }
}
