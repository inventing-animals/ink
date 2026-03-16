using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Ink.Data.Queries;
using Ink.Data.Results;

namespace Ink.Data.Sources;

/// <summary>
/// An in-memory <see cref="IDataSource{T}"/> backed by a fixed list.
/// Suitable for demos, tests, and small datasets that live entirely in memory.
/// Applies sort descriptors and filter conditions via reflection
/// (camelCase field name - PascalCase property).
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public sealed class ListDataSource<T> : IDataSource<T>
{
    private readonly IReadOnlyList<T> _items;

    public ListDataSource(IReadOnlyList<T> items) => _items = items;

    public event Action? Invalidated { add { } remove { } }

    public Task<DataPage<T>> QueryAsync(DataQuery query, CancellationToken ct = default)
    {
        IEnumerable<T> items = _items;

        // Apply filter
        if (query.Filter is not null)
            items = items.Where(item => EvaluateNode(item, query.Filter));

        var filtered = items.ToList();

        // Apply sort via reflection (camelCase field name - PascalCase property)
        IOrderedEnumerable<T>? ordered = null;

        foreach (var sort in query.Sort)
        {
            var propName = sort.Field.Length > 0
                ? char.ToUpperInvariant(sort.Field[0]) + sort.Field[1..]
                : sort.Field;

            var prop = typeof(T).GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
            if (prop is null) continue;

            Func<T, object?> key = item => prop.GetValue(item);

            ordered = (ordered, sort.Direction) switch
            {
                (null, SortDirection.Ascending)  => filtered.OrderBy(key),
                (null, SortDirection.Descending) => filtered.OrderByDescending(key),
                ({ }, SortDirection.Ascending)   => ordered!.ThenBy(key),
                ({ }, SortDirection.Descending)  => ordered!.ThenByDescending(key),
            };
        }

        IEnumerable<T> result = ordered is not null ? ordered : filtered;

        var skip = (query.Page - 1) * query.PageSize;
        var page = result.Skip(skip).Take(query.PageSize).ToList();
        return Task.FromResult(new DataPage<T>(page, filtered.Count));
    }

    private static bool EvaluateNode(T item, FilterNode node) => node switch
    {
        FilterAnd and   => and.Children.All(c => EvaluateNode(item, c)),
        FilterOr or     => or.Children.Any(c => EvaluateNode(item, c)),
        FilterNot not   => !EvaluateNode(item, not.Child),
        FilterCondition cond => EvaluateCondition(item, cond),
        _ => true,
    };

    private static bool EvaluateCondition(T item, FilterCondition cond)
    {
        var propName = cond.Field.Length > 0
            ? char.ToUpperInvariant(cond.Field[0]) + cond.Field[1..]
            : cond.Field;

        var prop = typeof(T).GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
        if (prop is null) return true;

        var value = prop.GetValue(item);
        var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

        if (cond.Op == FilterOp.IsNull)    return value is null;
        if (cond.Op == FilterOp.IsNotNull) return value is not null;

        if (cond.Op == FilterOp.In || cond.Op == FilterOp.NotIn)
        {
            var str = value?.ToString() ?? string.Empty;
            var matches = cond.Values.Any(v => string.Equals(str, v?.ToString(), StringComparison.OrdinalIgnoreCase));
            return cond.Op == FilterOp.In ? matches : !matches;
        }

        if (cond.Values.Count == 0) return true;
        var rawA = cond.Values[0]?.ToString() ?? string.Empty;

        if (cond.Op == FilterOp.Contains)
            return value?.ToString()?.Contains(rawA, StringComparison.OrdinalIgnoreCase) ?? false;
        if (cond.Op == FilterOp.StartsWith)
            return value?.ToString()?.StartsWith(rawA, StringComparison.OrdinalIgnoreCase) ?? false;
        if (cond.Op == FilterOp.EndsWith)
            return value?.ToString()?.EndsWith(rawA, StringComparison.OrdinalIgnoreCase) ?? false;

        if (cond.Op == FilterOp.Equal || cond.Op == FilterOp.NotEqual)
        {
            bool eq;
            if (propType == typeof(bool) && bool.TryParse(rawA, out var b))
                eq = Equals(value, b);
            else if (propType.IsEnum && Enum.TryParse(propType, rawA, ignoreCase: true, out var e))
                eq = Equals(value, e);
            else
                eq = string.Equals(value?.ToString(), rawA, StringComparison.OrdinalIgnoreCase);
            return cond.Op == FilterOp.Equal ? eq : !eq;
        }

        // Numeric / date range comparisons
        if (value is IComparable comparable)
        {
            var parsedA = ParseAs(rawA, propType);
            if (parsedA is null) return true;

            var cmpA = comparable.CompareTo(parsedA);

            if (cond.Op == FilterOp.LessThan)           return cmpA < 0;
            if (cond.Op == FilterOp.LessThanOrEqual)    return cmpA <= 0;
            if (cond.Op == FilterOp.GreaterThan)        return cmpA > 0;
            if (cond.Op == FilterOp.GreaterThanOrEqual) return cmpA >= 0;

            if (cond.Op == FilterOp.Between && cond.Values.Count >= 2)
            {
                var parsedB = ParseAs(cond.Values[1]?.ToString() ?? string.Empty, propType);
                return parsedB is not null && cmpA >= 0 && comparable.CompareTo(parsedB) <= 0;
            }
        }

        return true;
    }

    private static object? ParseAs(string raw, Type type)
    {
        if (type == typeof(int) && int.TryParse(raw, out var i)) return i;
        if (type == typeof(long) && long.TryParse(raw, out var l)) return l;
        if (type == typeof(short) && short.TryParse(raw, out var s)) return s;
        if (type == typeof(double) && double.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var d)) return d;
        if (type == typeof(float) && float.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var f)) return f;
        if (type == typeof(decimal) && decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var m)) return m;
        if (type == typeof(DateTime) && DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dt)) return dt;
        if (type == typeof(DateTimeOffset) && DateTimeOffset.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dto)) return dto;
        if (type == typeof(DateOnly) && DateOnly.TryParse(raw, CultureInfo.InvariantCulture, out var dn)) return dn;
        return null;
    }
}
