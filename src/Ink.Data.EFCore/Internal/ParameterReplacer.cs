using System.Linq.Expressions;

namespace Ink.Data.EFCore.Internal;

/// <summary>Rewrites a lambda body to use a different parameter expression.</summary>
internal sealed class ParameterReplacer : ExpressionVisitor
{
    private readonly ParameterExpression _from;
    private readonly ParameterExpression _to;

    private ParameterReplacer(ParameterExpression from, ParameterExpression to) =>
        (_from, _to) = (from, to);

    /// <summary>
    /// Replaces all occurrences of <paramref name="from"/> in <paramref name="body"/>
    /// with <paramref name="to"/>.
    /// </summary>
    public static Expression Replace(Expression body, ParameterExpression from, ParameterExpression to) =>
        new ParameterReplacer(from, to).Visit(body)!;

    /// <inheritdoc/>
    protected override Expression VisitParameter(ParameterExpression node) =>
        node == _from ? _to : base.VisitParameter(node);
}
