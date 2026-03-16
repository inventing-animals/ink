using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ink.Data.Queries;

/// <summary>
/// Base type for a composable filter expression tree.
/// Use <see cref="FilterAnd"/>, <see cref="FilterOr"/>, <see cref="FilterNot"/>,
/// or <see cref="FilterCondition"/> to build conditions.
/// The <c>$type</c> discriminator enables JSON round-tripping of the full tree.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(FilterAnd), "and")]
[JsonDerivedType(typeof(FilterOr), "or")]
[JsonDerivedType(typeof(FilterNot), "not")]
[JsonDerivedType(typeof(FilterCondition), "condition")]
public abstract record FilterNode;

/// <summary>All children must match.</summary>
/// <param name="Children">The child filter nodes.</param>
public sealed record FilterAnd(IReadOnlyList<FilterNode> Children) : FilterNode;

/// <summary>At least one child must match.</summary>
/// <param name="Children">The child filter nodes.</param>
public sealed record FilterOr(IReadOnlyList<FilterNode> Children) : FilterNode;

/// <summary>The child must not match.</summary>
/// <param name="Child">The negated filter node.</param>
public sealed record FilterNot(FilterNode Child) : FilterNode;

/// <summary>
/// A leaf condition comparing a column field against one or more values.
/// <para>
/// <paramref name="Values"/> interpretation by operator:
/// <list type="bullet">
///   <item><see cref="FilterOp.IsNull"/> / <see cref="FilterOp.IsNotNull"/> - empty list.</item>
///   <item>Most operators - single value at index 0.</item>
///   <item><see cref="FilterOp.Between"/> - two values: min at index 0, max at index 1.</item>
///   <item><see cref="FilterOp.In"/> / <see cref="FilterOp.NotIn"/> - one or more values.</item>
/// </list>
/// </para>
/// <para>
/// <paramref name="Field"/> is matched case-insensitively against registered columns.
/// </para>
/// </summary>
/// <param name="Field">The camelCase field name of the target column.</param>
/// <param name="Op">The filter operator.</param>
/// <param name="Values">The operand values.</param>
public sealed record FilterCondition(
    string Field,
    FilterOp Op,
    IReadOnlyList<object?> Values) : FilterNode;
