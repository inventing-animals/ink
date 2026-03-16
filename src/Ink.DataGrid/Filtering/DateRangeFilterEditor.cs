using System;
using System.Globalization;
using Avalonia.Controls;
using Ink.Data.Queries;

namespace Ink.DataGrid.Filtering;

/// <summary>
/// A filter editor for date/time columns. Renders two text inputs (From/To) that accept
/// dates in yyyy-MM-dd format and produces a <see cref="FilterOp.Between"/>,
/// <see cref="FilterOp.GreaterThanOrEqual"/>, or <see cref="FilterOp.LessThanOrEqual"/>
/// condition depending on which fields are filled.
/// </summary>
public sealed class DateRangeFilterEditor : IColumnFilterEditor
{
    private string _appliedFrom = string.Empty;
    private string _appliedTo   = string.Empty;
    private string _pendingFrom = string.Empty;
    private string _pendingTo   = string.Empty;

    public Control BuildControl()
    {
        _pendingFrom = _appliedFrom;
        _pendingTo   = _appliedTo;

        var fromBox = new TextBox { Text = _pendingFrom, Watermark = "From (yyyy-MM-dd)" };
        var toBox   = new TextBox { Text = _pendingTo,   Watermark = "To (yyyy-MM-dd)" };

        fromBox.TextChanged += (_, _) => _pendingFrom = fromBox.Text ?? string.Empty;
        toBox.TextChanged   += (_, _) => _pendingTo   = toBox.Text   ?? string.Empty;

        var panel = new StackPanel { Spacing = 8 };
        panel.Children.Add(fromBox);
        panel.Children.Add(toBox);
        return panel;
    }

    public FilterNode? BuildFilter(string fieldName)
    {
        var hasFrom = TryParse(_pendingFrom, out var from);
        var hasTo   = TryParse(_pendingTo, out var to);

        return (hasFrom, hasTo) switch
        {
            (true, true)   => new FilterCondition(fieldName, FilterOp.Between, [from.ToString("O", CultureInfo.InvariantCulture), to.ToString("O", CultureInfo.InvariantCulture)]),
            (true, false)  => new FilterCondition(fieldName, FilterOp.GreaterThanOrEqual, [from.ToString("O", CultureInfo.InvariantCulture)]),
            (false, true)  => new FilterCondition(fieldName, FilterOp.LessThanOrEqual,    [to.ToString("O", CultureInfo.InvariantCulture)]),
            _ => null,
        };
    }

    public void SetFilter(FilterNode? filter)
    {
        (_appliedFrom, _appliedTo) = filter switch
        {
            FilterCondition { Op: var op, Values: [string a, string b] } when op == FilterOp.Between =>
                (FormatBack(a), FormatBack(b)),
            FilterCondition { Op: var op, Values: [string v] } when op == FilterOp.GreaterThanOrEqual =>
                (FormatBack(v), string.Empty),
            FilterCondition { Op: var op, Values: [string v] } when op == FilterOp.LessThanOrEqual =>
                (string.Empty, FormatBack(v)),
            _ => (string.Empty, string.Empty),
        };

        _pendingFrom = _appliedFrom;
        _pendingTo   = _appliedTo;
    }

    private static bool TryParse(string s, out DateTime result)
    {
        if (string.IsNullOrWhiteSpace(s)) { result = default; return false; }
        return DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
    }

    private static string FormatBack(string iso) =>
        DateTime.TryParse(iso, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var d)
            ? d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            : string.Empty;
}
