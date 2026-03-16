using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Ink.Data.Columns;
using Ink.Data.Queries;
using Ink.DataGrid.Filtering;

namespace Ink.DataGrid.Columns;

/// <summary>
/// A DataGrid column with a header popup that supports sorting and filtering.
/// Automatically assigns a default <see cref="IColumnFilterEditor"/> based on
/// <typeparamref name="TValue"/>; set <see cref="FilterEditor"/> to override or
/// set it to <c>null</c> to disable the filter section.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
/// <typeparam name="TValue">The column value type.</typeparam>
/// <example>
/// <code>
/// Columns =
/// [
///     new DataGridColumn&lt;User, string&gt;(x => x.Name)      { Header = "Name" },
///     new DataGridColumn&lt;User, DateTime&gt;(x => x.Created) { Header = "Created" },
///     new DataGridColumn&lt;User, int&gt;(x => x.Age)          { Header = "Age", Sortable = false },
/// ];
/// </code>
/// </example>
public class DataGridColumn<TItem, TValue> : Column<TItem, TValue>, IDataGridColumn
{
    /// <inheritdoc/>
    public bool Sortable { get; init; } = true;

    /// <inheritdoc/>
    public IColumnFilterEditor? FilterEditor { get; init; }

    /// <summary>
    /// Initializes a new <see cref="DataGridColumn{TItem,TValue}"/> with a default filter editor
    /// inferred from <typeparamref name="TValue"/>.
    /// </summary>
    public DataGridColumn(Expression<Func<TItem, TValue>> selector, IReadOnlyList<FilterOp>? ops = null)
        : base(selector, ops)
    {
        FilterEditor = DefaultFilterEditor.For<TValue>();
    }
}
