using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ink.Data.Queries;

/// <summary>Extension methods for <see cref="FilterNode"/>.</summary>
public static class FilterNodeExtensions
{
    /// <summary>
    /// Combines a sequence of filter nodes with AND logic.
    /// Returns <c>null</c> when the sequence is empty, the single node when there is
    /// exactly one, and a <see cref="FilterAnd"/> when there are two or more.
    /// </summary>
    /// <param name="nodes">The nodes to combine.</param>
    public static FilterNode? And(this IEnumerable<FilterNode> nodes)
    {
        var list = nodes.ToList();
        return list.Count switch
        {
            0 => null,
            1 => list[0],
            _ => new FilterAnd(list),
        };
    }

    /// <summary>
    /// Combines a sequence of filter nodes with OR logic.
    /// Returns <c>null</c> when the sequence is empty, the single node when there is
    /// exactly one, and a <see cref="FilterOr"/> when there are two or more.
    /// </summary>
    /// <param name="nodes">The nodes to combine.</param>
    public static FilterNode? Or(this IEnumerable<FilterNode> nodes)
    {
        var list = nodes.ToList();
        return list.Count switch
        {
            0 => null,
            1 => list[0],
            _ => new FilterOr(list),
        };
    }

    /// <summary>
    /// Returns a human-readable indented string representation of the filter tree,
    /// suitable for logging and debugging.
    /// </summary>
    /// <example>
    /// <code>
    /// AND [
    ///   name contains "john"
    ///   OR [
    ///     status in ["Active", "Pending"]
    ///     age gt "18"
    ///   ]
    /// ]
    /// </code>
    /// </example>
    public static string ToDebugString(this FilterNode node) => Build(node, depth: 0);

    private static string Build(FilterNode node, int depth)
    {
        var pad = new string(' ', depth * 2);

        return node switch
        {
            FilterCondition c =>
                $"{pad}{c.Field} {c.Op} {FormatValues(c.Values)}",
            FilterAnd { Children: var children } =>
                BuildGroup("AND", children, pad, depth),
            FilterOr { Children: var children } =>
                BuildGroup("OR", children, pad, depth),
            FilterNot { Child: var child } =>
                $"{pad}NOT {Build(child, depth).TrimStart()}",
            _ => $"{pad}{node}",
        };
    }

    private static string BuildGroup(string op, IReadOnlyList<FilterNode> children, string pad, int depth)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{pad}{op} [");
        foreach (var child in children)
            sb.AppendLine(Build(child, depth + 1));
        sb.Append($"{pad}]");
        return sb.ToString();
    }

    private static string FormatValues(IReadOnlyList<object?> values) =>
        values.Count switch
        {
            0 => string.Empty,
            1 => $"\"{values[0]}\"",
            _ => $"[{string.Join(", ", values.Select(v => $"\"{v}\""))}]",
        };
}
