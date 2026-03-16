using System;
using System.Text.Json;
using Ink.Data.Queries;
using Xunit;

namespace Ink.Data.Tests.Queries;

public class FilterNodeJsonTests
{
    // Type discriminators

    [Theory]
    [InlineData("and")]
    [InlineData("or")]
    [InlineData("not")]
    [InlineData("condition")]
    public void Serialize_EachNodeType_HasCorrectTypeDiscriminator(string expectedType)
    {
        FilterNode node = expectedType switch
        {
            "and" => new FilterAnd([new FilterCondition("x", FilterOp.IsNull, [])]),
            "or" => new FilterOr([new FilterCondition("x", FilterOp.IsNull, [])]),
            "not" => new FilterNot(new FilterCondition("x", FilterOp.IsNull, [])),
            "condition" => new FilterCondition("x", FilterOp.IsNull, []),
            _ => throw new ArgumentOutOfRangeException(),
        };

        var json = JsonSerializer.Serialize<FilterNode>(node);
        Assert.Contains($"\"$type\":\"{expectedType}\"", json);
    }

    // FilterOp serializes as plain string

    [Fact]
    public void Serialize_FilterOp_IsStringNotObject()
    {
        var cond = new FilterCondition("age", FilterOp.GreaterThan, [18]);
        var json = JsonSerializer.Serialize<FilterNode>(cond);

        Assert.Contains("\"gt\"", json);
        Assert.DoesNotContain("\"Value\"", json);   // must not serialize as { "Value": "gt" }
    }

    // Round-trips

    [Fact]
    public void RoundTrip_FilterCondition_PreservesFieldAndOp()
    {
        FilterNode original = new FilterCondition("email", FilterOp.EndsWith, ["@example.com"]);
        var json = JsonSerializer.Serialize<FilterNode>(original);
        var cond = Assert.IsType<FilterCondition>(JsonSerializer.Deserialize<FilterNode>(json));

        Assert.Equal("email", cond.Field);
        Assert.Equal(FilterOp.EndsWith, cond.Op);
    }

    [Fact]
    public void RoundTrip_FilterAnd_PreservesChildCount()
    {
        FilterNode original = new FilterAnd([
            new FilterCondition("name", FilterOp.Contains, ["alice"]),
            new FilterCondition("age", FilterOp.GreaterThan, [18]),
        ]);
        var json = JsonSerializer.Serialize<FilterNode>(original);
        var and = Assert.IsType<FilterAnd>(JsonSerializer.Deserialize<FilterNode>(json));

        Assert.Equal(2, and.Children.Count);
        Assert.IsType<FilterCondition>(and.Children[0]);
        Assert.IsType<FilterCondition>(and.Children[1]);
    }

    [Fact]
    public void RoundTrip_NestedTree_PreservesFullStructure()
    {
        FilterNode original = new FilterAnd([
            new FilterCondition("name", FilterOp.Contains, ["alice"]),
            new FilterOr([
                new FilterCondition("role", FilterOp.Equal, ["Admin"]),
                new FilterNot(new FilterCondition("archived", FilterOp.Equal, [true])),
            ]),
        ]);

        var json = JsonSerializer.Serialize<FilterNode>(original);
        var and = Assert.IsType<FilterAnd>(JsonSerializer.Deserialize<FilterNode>(json));
        var or = Assert.IsType<FilterOr>(and.Children[1]);
        Assert.IsType<FilterNot>(or.Children[1]);
    }

    // Values come back as JsonElement - TypeCoercion in Ink.Data.EFCore handles conversion

    [Fact]
    public void RoundTrip_Values_AreDeserializedAsJsonElements()
    {
        FilterNode original = new FilterCondition("age", FilterOp.Between, [18, 65]);
        var json = JsonSerializer.Serialize<FilterNode>(original);
        var cond = Assert.IsType<FilterCondition>(JsonSerializer.Deserialize<FilterNode>(json));

        Assert.Equal(2, cond.Values.Count);
        // Values arrive as JsonElement after JSON round-trip; EFCoreQueryTranslator coerces them
        Assert.IsType<JsonElement>(cond.Values[0]);
        Assert.IsType<JsonElement>(cond.Values[1]);
    }

    // Full JSON document snapshots

    [Fact]
    public void Serialize_FilterCondition_MatchesDocument()
    {
        FilterNode node = new FilterCondition("age", FilterOp.GreaterThan, [18]);
        var json = JsonSerializer.Serialize<FilterNode>(node);

        Assert.Equal(
            "{\"$type\":\"condition\",\"Field\":\"age\",\"Op\":\"gt\",\"Values\":[18]}",
            json);
    }

    [Fact]
    public void Serialize_FilterAnd_MatchesDocument()
    {
        FilterNode node = new FilterAnd([
            new FilterCondition("name", FilterOp.Contains, ["alice"]),
            new FilterCondition("age", FilterOp.GreaterThan, [18]),
        ]);
        var json = JsonSerializer.Serialize<FilterNode>(node);

        Assert.Equal(
            "{\"$type\":\"and\",\"Children\":[" +
                "{\"$type\":\"condition\",\"Field\":\"name\",\"Op\":\"contains\",\"Values\":[\"alice\"]}," +
                "{\"$type\":\"condition\",\"Field\":\"age\",\"Op\":\"gt\",\"Values\":[18]}" +
            "]}",
            json);
    }

    [Fact]
    public void Serialize_NestedTree_MatchesDocument()
    {
        FilterNode node = new FilterAnd([
            new FilterCondition("name", FilterOp.Contains, ["alice"]),
            new FilterOr([
                new FilterCondition("role", FilterOp.Equal, ["Admin"]),
                new FilterNot(new FilterCondition("archived", FilterOp.Equal, [true])),
            ]),
        ]);
        var json = JsonSerializer.Serialize<FilterNode>(node);

        Assert.Equal(
            "{\"$type\":\"and\",\"Children\":[" +
                "{\"$type\":\"condition\",\"Field\":\"name\",\"Op\":\"contains\",\"Values\":[\"alice\"]}," +
                "{\"$type\":\"or\",\"Children\":[" +
                    "{\"$type\":\"condition\",\"Field\":\"role\",\"Op\":\"eq\",\"Values\":[\"Admin\"]}," +
                    "{\"$type\":\"not\",\"Child\":" +
                        "{\"$type\":\"condition\",\"Field\":\"archived\",\"Op\":\"eq\",\"Values\":[true]}" +
                    "}" +
                "]}" +
            "]}",
            json);
    }

    // Custom ops round-trip naturally since FilterOp is just a string

    [Fact]
    public void RoundTrip_CustomOp_PreservesOpValue()
    {
        var fts = new FilterOp("fts");
        FilterNode original = new FilterCondition("description", fts, ["search term"]);

        var json = JsonSerializer.Serialize<FilterNode>(original);
        Assert.Contains("\"fts\"", json);

        var cond = Assert.IsType<FilterCondition>(JsonSerializer.Deserialize<FilterNode>(json));
        Assert.Equal(fts, cond.Op);
    }
}
