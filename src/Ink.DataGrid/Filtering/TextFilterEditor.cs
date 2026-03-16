using Avalonia.Controls;
using Ink.Data.Queries;

namespace Ink.DataGrid.Filtering;

/// <summary>
/// A filter editor for string columns. Renders a single text input and produces a
/// <see cref="FilterOp.Contains"/> condition.
/// </summary>
public sealed class TextFilterEditor : IColumnFilterEditor
{
    private string _applied = string.Empty;
    private string _pending = string.Empty;

    public Control BuildControl()
    {
        _pending = _applied;

        var tb = new TextBox
        {
            Text = _pending,
            Watermark = "Search...",
        };

        tb.TextChanged += (_, _) => _pending = tb.Text ?? string.Empty;

        return tb;
    }

    public FilterNode? BuildFilter(string fieldName) =>
        string.IsNullOrWhiteSpace(_pending)
            ? null
            : new FilterCondition(fieldName, FilterOp.Contains, [_pending.Trim()]);

    public void SetFilter(FilterNode? filter)
    {
        _applied = filter is FilterCondition { Values: [string v] } ? v : string.Empty;
        _pending = _applied;
    }
}
