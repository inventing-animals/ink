using System;
using System.Collections.Generic;
using Ink.Data.Queries;

namespace Ink.Data.Columns;

/// <summary>
/// Provides sensible default <see cref="FilterOp"/> sets inferred from a column's value type.
/// </summary>
public static class DefaultOps
{
    private static readonly IReadOnlyList<FilterOp> StringOps =
    [
        FilterOp.Equal, FilterOp.NotEqual,
        FilterOp.Contains, FilterOp.StartsWith, FilterOp.EndsWith,
        FilterOp.IsNull, FilterOp.IsNotNull,
    ];

    private static readonly IReadOnlyList<FilterOp> ComparableOps =
    [
        FilterOp.Equal, FilterOp.NotEqual,
        FilterOp.LessThan, FilterOp.LessThanOrEqual,
        FilterOp.GreaterThan, FilterOp.GreaterThanOrEqual,
        FilterOp.Between, FilterOp.IsNull, FilterOp.IsNotNull,
    ];

    private static readonly IReadOnlyList<FilterOp> BoolOps =
    [
        FilterOp.Equal, FilterOp.NotEqual,
    ];

    private static readonly IReadOnlyList<FilterOp> EnumOps =
    [
        FilterOp.Equal, FilterOp.NotEqual, FilterOp.In, FilterOp.NotIn,
    ];

    private static readonly IReadOnlyList<FilterOp> FallbackOps =
    [
        FilterOp.Equal, FilterOp.NotEqual, FilterOp.IsNull, FilterOp.IsNotNull,
    ];

    /// <summary>
    /// Returns the default filter operations for <typeparamref name="T"/>.
    /// Strips nullable wrappers before matching.
    /// </summary>
    public static IReadOnlyList<FilterOp> For<T>()
    {
        var t = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        if (t == typeof(string)) return StringOps;
        if (t == typeof(bool)) return BoolOps;
        if (t.IsEnum) return EnumOps;
        if (typeof(IComparable).IsAssignableFrom(t)) return ComparableOps;

        return FallbackOps;
    }
}
