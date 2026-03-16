using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Ink.Data.Columns;
using Ink.Data.Queries;

namespace Ink.Data.EFCore.Internal;

/// <summary>Builds LINQ predicate expressions from <see cref="FilterNode"/> trees.</summary>
internal static class FilterExpressionBuilder
{
    /// <summary>
    /// Builds an <c>Expression&lt;Func&lt;T, bool&gt;&gt;</c> from the given filter tree.
    /// </summary>
    public static Expression<Func<T, bool>> Build<T>(
        FilterNode root,
        IReadOnlyDictionary<string, Column<T>> columns,
        IReadOnlyDictionary<FilterOp, Func<Expression, Type, IReadOnlyList<object?>, Expression>> customOps)
    {
        var param = Expression.Parameter(typeof(T), "x");
        var body = BuildNode(root, param, columns, customOps);
        return Expression.Lambda<Func<T, bool>>(body, param);
    }

    private static Expression BuildNode<T>(
        FilterNode node,
        ParameterExpression param,
        IReadOnlyDictionary<string, Column<T>> columns,
        IReadOnlyDictionary<FilterOp, Func<Expression, Type, IReadOnlyList<object?>, Expression>> customOps) =>
        node switch
        {
            FilterAnd and => and.Children
                .Select(c => BuildNode(c, param, columns, customOps))
                .Aggregate(Expression.AndAlso),
            FilterOr or => or.Children
                .Select(c => BuildNode(c, param, columns, customOps))
                .Aggregate(Expression.OrElse),
            FilterNot not => Expression.Not(BuildNode(not.Child, param, columns, customOps)),
            FilterCondition cond => BuildCondition(cond, param, columns, customOps),
            _ => throw new ArgumentException($"Unknown filter node type: {node.GetType().Name}"),
        };

    private static Expression BuildCondition<T>(
        FilterCondition condition,
        ParameterExpression param,
        IReadOnlyDictionary<string, Column<T>> columns,
        IReadOnlyDictionary<FilterOp, Func<Expression, Type, IReadOnlyList<object?>, Expression>> customOps)
    {
        var col = columns[condition.Field];
        var memberAccess = ParameterReplacer.Replace(
            col.SelectorExpression.Body,
            col.SelectorExpression.Parameters[0],
            param);
        var memberType = memberAccess.Type;

        // Custom ops registered on the translator take precedence over built-ins.
        if (customOps.TryGetValue(condition.Op, out var customHandler))
            return customHandler(memberAccess, memberType, condition.Values);

        return condition.Op.Value switch
        {
            "isNull" =>
                Expression.Equal(memberAccess, Expression.Constant(null, memberType)),
            "isNotNull" =>
                Expression.NotEqual(memberAccess, Expression.Constant(null, memberType)),
            "eq" =>
                BuildBinary(memberAccess, memberType, condition.Values[0], Expression.Equal),
            "neq" =>
                BuildBinary(memberAccess, memberType, condition.Values[0], Expression.NotEqual),
            "lt" =>
                BuildBinary(memberAccess, memberType, condition.Values[0], Expression.LessThan),
            "lte" =>
                BuildBinary(memberAccess, memberType, condition.Values[0], Expression.LessThanOrEqual),
            "gt" =>
                BuildBinary(memberAccess, memberType, condition.Values[0], Expression.GreaterThan),
            "gte" =>
                BuildBinary(memberAccess, memberType, condition.Values[0], Expression.GreaterThanOrEqual),
            "contains" =>
                BuildStringMethod(memberAccess, condition.Values[0], nameof(string.Contains)),
            "startsWith" =>
                BuildStringMethod(memberAccess, condition.Values[0], nameof(string.StartsWith)),
            "endsWith" =>
                BuildStringMethod(memberAccess, condition.Values[0], nameof(string.EndsWith)),
            "in" =>
                BuildIn(memberAccess, memberType, condition.Values, negate: false),
            "notIn" =>
                BuildIn(memberAccess, memberType, condition.Values, negate: true),
            "between" =>
                BuildBetween(memberAccess, memberType, condition.Values),
            _ => throw new ArgumentException(
                $"Unsupported filter operation '{condition.Op}'. " +
                $"Register a custom handler via translator.HandleOp(new FilterOp(\"{condition.Op}\"), ...)."),
        };
    }

    private static Expression BuildBinary(
        Expression memberAccess,
        Type memberType,
        object? rawValue,
        Func<Expression, Expression, BinaryExpression> factory)
    {
        var value = TypeCoercion.Coerce(rawValue, memberType);
        return factory(memberAccess, Expression.Constant(value, memberType));
    }

    private static Expression BuildStringMethod(
        Expression memberAccess,
        object? rawValue,
        string methodName)
    {
        var method = typeof(string).GetMethod(methodName, [typeof(string)])!;
        var value = (string?)TypeCoercion.Coerce(rawValue, typeof(string));
        return Expression.Call(memberAccess, method, Expression.Constant(value, typeof(string)));
    }

    private static Expression BuildIn(
        Expression memberAccess,
        Type memberType,
        IReadOnlyList<object?> rawValues,
        bool negate)
    {
        // Build a List<TValue> at runtime so EF Core translates it to SQL IN (...)
        var listType = typeof(List<>).MakeGenericType(memberType);
        var list = (IList)Activator.CreateInstance(listType)!;
        foreach (var v in rawValues)
            list.Add(TypeCoercion.Coerce(v, memberType));

        var containsMethod = listType.GetMethod(nameof(List<object>.Contains), [memberType])!;
        var listConst = Expression.Constant(list, listType);
        Expression result = Expression.Call(listConst, containsMethod, memberAccess);
        return negate ? Expression.Not(result) : result;
    }

    private static Expression BuildBetween(
        Expression memberAccess,
        Type memberType,
        IReadOnlyList<object?> values)
    {
        var min = TypeCoercion.Coerce(values[0], memberType);
        var max = TypeCoercion.Coerce(values[1], memberType);
        return Expression.AndAlso(
            Expression.GreaterThanOrEqual(memberAccess, Expression.Constant(min, memberType)),
            Expression.LessThanOrEqual(memberAccess, Expression.Constant(max, memberType)));
    }
}
