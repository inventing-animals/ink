using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Ink.Data.Queries;

namespace Ink.DataGrid.Filtering;

/// <summary>
/// A filter editor for enum columns. Renders a checkbox list of all enum values and produces
/// a <see cref="FilterOp.In"/> condition for the selected items.
/// </summary>
public sealed class SelectionFilterEditor : IColumnFilterEditor
{
    private readonly Type _enumType;
    private readonly string[] _names;
    private HashSet<string> _applied = [];
    private HashSet<string> _pending = [];

    public SelectionFilterEditor(Type enumType)
    {
        _enumType = enumType;
        _names    = Enum.GetNames(enumType);
    }

    public Control BuildControl()
    {
        _pending = new HashSet<string>(_applied, StringComparer.Ordinal);

        var panel = new StackPanel { Spacing = 4 };

        foreach (var name in _names)
        {
            var cb = new CheckBox { Content = name, IsChecked = _pending.Contains(name) };

            cb.IsCheckedChanged += (_, _) =>
            {
                if (cb.IsChecked == true) _pending.Add(name);
                else _pending.Remove(name);
            };

            panel.Children.Add(cb);
        }

        return panel;
    }

    public FilterNode? BuildFilter(string fieldName) =>
        _pending.Count == 0
            ? null
            : new FilterCondition(fieldName, FilterOp.In, [.. _pending.Cast<object?>()]);

    public void SetFilter(FilterNode? filter)
    {
        _applied.Clear();

        if (filter is FilterCondition { Op: var op, Values: var values } && op == FilterOp.In)
            foreach (var v in values.OfType<string>())
                _applied.Add(v);

        _pending = new HashSet<string>(_applied, StringComparer.Ordinal);
    }
}
