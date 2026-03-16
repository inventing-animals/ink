using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Ink.Data.Columns;
using Ink.Data.Queries;

namespace Ink.Data.EFCore.Internal;

/// <summary>
/// Applies <see cref="SortDescriptor"/> entries to an <see cref="IQueryable{T}"/>
/// by dynamically invoking <c>OrderBy</c> / <c>ThenBy</c> through reflection.
/// </summary>
internal static class SortExpressionBuilder
{
    // Cache method infos to avoid repeated reflection lookups.
    private static readonly MethodInfo OrderByMethod =
        typeof(Queryable).GetMethods().First(m => m.Name == nameof(Queryable.OrderBy) && m.GetParameters().Length == 2);

    private static readonly MethodInfo OrderByDescendingMethod =
        typeof(Queryable).GetMethods().First(m => m.Name == nameof(Queryable.OrderByDescending) && m.GetParameters().Length == 2);

    private static readonly MethodInfo ThenByMethod =
        typeof(Queryable).GetMethods().First(m => m.Name == nameof(Queryable.ThenBy) && m.GetParameters().Length == 2);

    private static readonly MethodInfo ThenByDescendingMethod =
        typeof(Queryable).GetMethods().First(m => m.Name == nameof(Queryable.ThenByDescending) && m.GetParameters().Length == 2);

    /// <summary>Applies all sort descriptors to <paramref name="source"/> in order.</summary>
    public static IQueryable<T> Apply<T>(
        IQueryable<T> source,
        IReadOnlyList<SortDescriptor> sort,
        IReadOnlyDictionary<string, Column<T>> columns)
    {
        IQueryable<T> result = source;
        var isFirst = true;

        foreach (var descriptor in sort)
        {
            var col = columns[descriptor.Field];
            var param = Expression.Parameter(typeof(T), "x");
            var memberAccess = ParameterReplacer.Replace(
                col.SelectorExpression.Body,
                col.SelectorExpression.Parameters[0],
                param);
            var keyType = memberAccess.Type;
            var keySelector = Expression.Lambda(memberAccess, param);

            var baseMethod = isFirst
                ? (descriptor.Direction == SortDirection.Ascending ? OrderByMethod : OrderByDescendingMethod)
                : (descriptor.Direction == SortDirection.Ascending ? ThenByMethod : ThenByDescendingMethod);

            var method = baseMethod.MakeGenericMethod(typeof(T), keyType);
            result = (IQueryable<T>)method.Invoke(null, [result, keySelector])!;
            isFirst = false;
        }

        return result;
    }
}
