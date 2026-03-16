using Avalonia.Controls;
using Ink.Data.Queries;

namespace Ink.DataGrid.Filtering;

/// <summary>
/// A filter editor for numeric columns. Renders Min and Max inputs and produces a
/// <see cref="FilterOp.Between"/>, <see cref="FilterOp.GreaterThanOrEqual"/>, or
/// <see cref="FilterOp.LessThanOrEqual"/> condition depending on which fields are filled.
/// </summary>
public sealed class NumericRangeFilterEditor : IColumnFilterEditor
{
    private string _appliedMin = string.Empty;
    private string _appliedMax = string.Empty;
    private string _pendingMin = string.Empty;
    private string _pendingMax = string.Empty;

    public Control BuildControl()
    {
        _pendingMin = _appliedMin;
        _pendingMax = _appliedMax;

        var minBox = new TextBox { Text = _pendingMin, Watermark = "Min" };
        var maxBox = new TextBox { Text = _pendingMax, Watermark = "Max" };

        minBox.TextChanged += (_, _) => _pendingMin = minBox.Text ?? string.Empty;
        maxBox.TextChanged += (_, _) => _pendingMax = maxBox.Text ?? string.Empty;

        var panel = new StackPanel { Spacing = 8 };
        panel.Children.Add(minBox);
        panel.Children.Add(maxBox);
        return panel;
    }

    public FilterNode? BuildFilter(string fieldName)
    {
        var hasMin = !string.IsNullOrWhiteSpace(_pendingMin);
        var hasMax = !string.IsNullOrWhiteSpace(_pendingMax);

        return (hasMin, hasMax) switch
        {
            (true, true)   => new FilterCondition(fieldName, FilterOp.Between,             [_pendingMin.Trim(), _pendingMax.Trim()]),
            (true, false)  => new FilterCondition(fieldName, FilterOp.GreaterThanOrEqual,  [_pendingMin.Trim()]),
            (false, true)  => new FilterCondition(fieldName, FilterOp.LessThanOrEqual,     [_pendingMax.Trim()]),
            _              => null,
        };
    }

    public void SetFilter(FilterNode? filter)
    {
        (_appliedMin, _appliedMax) = filter switch
        {
            FilterCondition { Op: var op, Values: [string a, string b] } when op == FilterOp.Between            => (a, b),
            FilterCondition { Op: var op, Values: [string v] }           when op == FilterOp.GreaterThanOrEqual => (v, string.Empty),
            FilterCondition { Op: var op, Values: [string v] }           when op == FilterOp.LessThanOrEqual    => (string.Empty, v),
            _                                                                                                    => (string.Empty, string.Empty),
        };

        _pendingMin = _appliedMin;
        _pendingMax = _appliedMax;
    }
}
