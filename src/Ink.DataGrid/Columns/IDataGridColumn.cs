using Ink.Data.Columns;
using Ink.DataGrid.Filtering;

namespace Ink.DataGrid.Columns;

/// <summary>
/// Extends <see cref="IColumn"/> with DataGrid-specific capabilities: sorting and filter editors.
/// Use <see cref="DataGridColumn{TItem,TValue}"/> as the concrete implementation.
/// Plain <see cref="Ink.Data.Columns.Column{TItem,TValue}"/> instances are also accepted by the
/// DataGrid but render without a column header popup.
/// </summary>
public interface IDataGridColumn : IColumn
{
    /// <summary>Gets whether this column can be sorted.</summary>
    bool Sortable { get; }

    /// <summary>
    /// Gets the filter editor shown in the column header popup, or <c>null</c> to omit
    /// the filter section entirely.
    /// </summary>
    IColumnFilterEditor? FilterEditor { get; }
}
