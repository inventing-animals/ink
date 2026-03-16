using System.Text.Json.Serialization;

namespace Ink.Data.Queries;

/// <summary>Sort direction for a column.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SortDirection
{
    /// <summary>Ascending order (A → Z, 0 → 9).</summary>
    Ascending,

    /// <summary>Descending order (Z → A, 9 → 0).</summary>
    Descending,
}

/// <summary>Describes a single sort criterion.</summary>
/// <param name="Field">The camelCase field name of the column to sort by.</param>
/// <param name="Direction">The sort direction.</param>
public sealed record SortDescriptor(string Field, SortDirection Direction);
