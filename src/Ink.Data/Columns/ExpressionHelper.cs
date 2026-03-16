using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Ink.Data.Columns;

/// <summary>Utilities for extracting member metadata from selector expressions.</summary>
public static class ExpressionHelper
{
    /// <summary>
    /// Extracts the member name from a simple member-access lambda expression.
    /// </summary>
    /// <param name="expression">A lambda of the form <c>x => x.Property</c>.</param>
    /// <returns>The member name, e.g. <c>"CreatedAt"</c>.</returns>
    /// <exception cref="ArgumentException">Thrown when the expression is not a member access.</exception>
    public static string GetMemberName(LambdaExpression expression) =>
        expression.Body switch
        {
            MemberExpression member => member.Member.Name,
            UnaryExpression { Operand: MemberExpression member } => member.Member.Name,
            _ => throw new ArgumentException(
                $"Expression body must be a member access, got: {expression.Body.NodeType}",
                nameof(expression)),
        };

    /// <summary>
    /// Returns the member name in camelCase, e.g. <c>x => x.CreatedAt</c> → <c>"createdAt"</c>.
    /// </summary>
    public static string ToCamelCase(LambdaExpression expression)
    {
        var name = GetMemberName(expression);
        return name.Length == 0 ? name : char.ToLowerInvariant(name[0]) + name[1..];
    }

    /// <summary>Gets the <see cref="MemberInfo"/> from a member-access lambda expression.</summary>
    public static MemberInfo GetMemberInfo(LambdaExpression expression) =>
        expression.Body switch
        {
            MemberExpression member => member.Member,
            UnaryExpression { Operand: MemberExpression member } => member.Member,
            _ => throw new ArgumentException(
                $"Expression body must be a member access, got: {expression.Body.NodeType}",
                nameof(expression)),
        };
}
