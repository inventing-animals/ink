using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Ink.Data.Queries;

namespace Ink.Data.Columns;

/// <summary>
/// Non-generic base for a column definition.
/// Exposes the field name, supported filter operations, and the untyped selector expression
/// so that query translators can work without knowing <c>TValue</c>.
/// </summary>
/// <typeparam name="TItem">The item type this column belongs to.</typeparam>
public abstract class Column<TItem>
{
    /// <summary>Gets the camelCase field name derived from the selector expression.</summary>
    public abstract string FieldName { get; }

    /// <summary>Gets the filter operations supported by this column.</summary>
    public abstract IReadOnlyList<FilterOp> SupportedOps { get; }

    /// <summary>
    /// Gets the untyped selector lambda used by query translators.
    /// The body is a member access on <c>TItem</c>.
    /// </summary>
    public abstract LambdaExpression SelectorExpression { get; }
}

/// <summary>
/// A typed column definition with a strongly-typed selector expression.
/// The field name is derived from the expression and never written by hand.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
/// <typeparam name="TValue">The column value type.</typeparam>
/// <example>
/// <code>
/// var nameColumn = new Column&lt;User, string&gt;(x => x.Name);
/// var createdColumn = new Column&lt;User, DateTimeOffset&gt;(x => x.CreatedAt);
/// </code>
/// </example>
public class Column<TItem, TValue> : Column<TItem>
{
    /// <summary>
    /// Initializes a new <see cref="Column{TItem,TValue}"/>.
    /// </summary>
    /// <param name="selector">Member-access expression selecting the column value, e.g. <c>x => x.Name</c>.</param>
    /// <param name="ops">
    /// Override the supported filter operations.
    /// When <c>null</c>, defaults are inferred from <typeparamref name="TValue"/> via <see cref="DefaultOps"/>.
    /// </param>
    public Column(Expression<Func<TItem, TValue>> selector, IReadOnlyList<FilterOp>? ops = null)
    {
        Selector = selector;
        SupportedOps = ops ?? DefaultOps.For<TValue>();
    }

    /// <summary>Gets the typed selector expression.</summary>
    public Expression<Func<TItem, TValue>> Selector { get; }

    /// <inheritdoc/>
    public override string FieldName => ExpressionHelper.ToCamelCase(Selector);

    /// <inheritdoc/>
    public override IReadOnlyList<FilterOp> SupportedOps { get; }

    /// <inheritdoc/>
    public override LambdaExpression SelectorExpression => Selector;
}
