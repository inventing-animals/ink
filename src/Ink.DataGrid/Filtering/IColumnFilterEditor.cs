using Avalonia.Controls;
using Ink.Data.Queries;

namespace Ink.DataGrid.Filtering;

/// <summary>
/// Builds the Avalonia control shown in a column header popup's filter section,
/// and converts the control's state into a <see cref="FilterNode"/>.
/// </summary>
public interface IColumnFilterEditor
{
    /// <summary>
    /// Creates a fresh Avalonia control for the current filter state.
    /// Called each time the column header popup opens.
    /// The returned control writes user input back to the editor's pending state.
    /// </summary>
    Control BuildControl();

    /// <summary>
    /// Returns a <see cref="FilterNode"/> representing the current pending state,
    /// or <c>null</c> if no filter should be applied.
    /// Called when the user clicks Apply in the popup.
    /// </summary>
    FilterNode? BuildFilter(string fieldName);

    /// <summary>
    /// Updates the editor's applied state. Called after Apply (with the produced filter)
    /// and after Clear (with <c>null</c>). The next <see cref="BuildControl"/> call
    /// will pre-populate the control from this state.
    /// </summary>
    void SetFilter(FilterNode? filter);
}
