using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ink.Data.Queries;

/// <summary>
/// Identifies a filter comparison operation. Defined as a value type wrapping a string
/// so that host applications can introduce custom operators without modifying this library.
/// </summary>
/// <example>
/// Built-in: <c>FilterOp.Contains</c>, <c>FilterOp.GreaterThan</c>, etc.
/// Custom:   <c>new FilterOp("fts")</c> for full-text search.
/// </example>
[JsonConverter(typeof(FilterOpJsonConverter))]
public readonly record struct FilterOp(string Value)
{
    // Equality
    /// <summary>Value equals the operand.</summary>
    public static readonly FilterOp Equal = new("eq");

    /// <summary>Value does not equal the operand.</summary>
    public static readonly FilterOp NotEqual = new("neq");

    // String
    /// <summary>String value contains the operand (case-insensitive on most databases).</summary>
    public static readonly FilterOp Contains = new("contains");

    /// <summary>String value starts with the operand.</summary>
    public static readonly FilterOp StartsWith = new("startsWith");

    /// <summary>String value ends with the operand.</summary>
    public static readonly FilterOp EndsWith = new("endsWith");

    // Comparison
    /// <summary>Value is less than the operand.</summary>
    public static readonly FilterOp LessThan = new("lt");

    /// <summary>Value is less than or equal to the operand.</summary>
    public static readonly FilterOp LessThanOrEqual = new("lte");

    /// <summary>Value is greater than the operand.</summary>
    public static readonly FilterOp GreaterThan = new("gt");

    /// <summary>Value is greater than or equal to the operand.</summary>
    public static readonly FilterOp GreaterThanOrEqual = new("gte");

    // Set
    /// <summary>Value is contained in the operand list.</summary>
    public static readonly FilterOp In = new("in");

    /// <summary>Value is not contained in the operand list.</summary>
    public static readonly FilterOp NotIn = new("notIn");

    // Range
    /// <summary>Value is between <c>Values[0]</c> and <c>Values[1]</c> inclusive.</summary>
    public static readonly FilterOp Between = new("between");

    // Null
    /// <summary>Value is null.</summary>
    public static readonly FilterOp IsNull = new("isNull");

    /// <summary>Value is not null.</summary>
    public static readonly FilterOp IsNotNull = new("isNotNull");

    /// <inheritdoc/>
    public override string ToString() => Value;
}

internal sealed class FilterOpJsonConverter : JsonConverter<FilterOp>
{
    public override FilterOp Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        new(reader.GetString() ?? string.Empty);

    public override void Write(Utf8JsonWriter writer, FilterOp value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}
