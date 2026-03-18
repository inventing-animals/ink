using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Threading;
using Ink.Data.Columns;
using Ink.Data.Queries;
using Ink.DataGrid.Columns;
using InkFlyout = Ink.UI.Controls.Flyout;

namespace Ink.DataGrid.Controls;

public class DataGrid : TemplatedControl
{
    public static readonly StyledProperty<DataGridModel?> ModelProperty =
        AvaloniaProperty.Register<DataGrid, DataGridModel?>(nameof(Model));

    public DataGridModel? Model
    {
        get => GetValue(ModelProperty);
        set => SetValue(ModelProperty, value);
    }

    private Panel? _headerPanel;
    private Panel? _body;
    private CancellationTokenSource? _cts;

    // Sort state - list order determines multi-sort priority
    private readonly List<SortDescriptor> _sort = [];

    // Filter state - one active node per column field
    private readonly Dictionary<string, FilterNode> _activeFilters = [];

    // Sort indicator TextBlocks keyed by field name, refreshed together on any sort change
    private readonly Dictionary<string, TextBlock> _sortIndicators = [];

    static DataGrid()
    {
        ModelProperty.Changed.AddClassHandler<DataGrid>((grid, _) => grid.OnModelChanged());
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _headerPanel = e.NameScope.Find<Panel>("PART_Header");
        _body        = e.NameScope.Find<Panel>("PART_Body");
        OnModelChanged();
    }

    private void OnModelChanged()
    {
        _sort.Clear();
        _activeFilters.Clear();
        _sortIndicators.Clear();
        BuildHeader();
        LoadRows();
    }

    // -------------------------------------------------------------------------
    // Header
    // -------------------------------------------------------------------------

    private void BuildHeader()
    {
        if (_headerPanel is null) return;

        _headerPanel.Children.Clear();
        _sortIndicators.Clear();

        var columns = Model?.Columns;
        if (columns is null || columns.Count == 0) return;

        var grid = BuildColumnGrid(columns);

        for (var i = 0; i < columns.Count; i++)
        {
            var col  = columns[i];
            var cell = col is IDataGridColumn dgCol
                ? BuildInteractiveHeaderCell(dgCol)
                : BuildPlainHeaderCell(col);

            Grid.SetColumn(cell, i);
            grid.Children.Add(cell);
        }

        _headerPanel.Children.Add(grid);
    }

    private static Control BuildPlainHeaderCell(IColumn col)
    {
        var cell = new Border();
        cell.Classes.Add("ink-datagrid-header-cell");

        var text = new TextBlock { Text = col.Header };
        text.Classes.Add("ink-datagrid-header-text");

        cell.Child = text;
        return cell;
    }

    private Control BuildInteractiveHeaderCell(IDataGridColumn col)
    {
        var sortIndicator = new TextBlock();
        sortIndicator.Classes.Add("ink-datagrid-sort-indicator");
        _sortIndicators[col.FieldName] = sortIndicator;

        var nameText = new TextBlock { Text = col.Header };
        nameText.Classes.Add("ink-datagrid-header-text");

        var content = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4 };
        content.Children.Add(nameText);
        content.Children.Add(sortIndicator);

        var btn = new Button
        {
            Content                    = content,
            HorizontalAlignment        = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            Padding                    = new Thickness(0),
        };
        btn.Classes.Add("ink-ghost");

        btn.Click += (_, _) =>
        {
            UpdateAllSortIndicators();
            BuildColumnFlyout(col).ShowAt(btn);
        };

        var cell = new Border();
        cell.Classes.Add("ink-datagrid-header-cell");
        cell.Classes.Add("ink-datagrid-header-cell-interactive");
        cell.Child = btn;
        return cell;
    }

    // -------------------------------------------------------------------------
    // Column popup
    // -------------------------------------------------------------------------

    private InkFlyout BuildColumnFlyout(IDataGridColumn col)
    {
        var flyout = new InkFlyout();
        var stack  = new StackPanel { Spacing = 0, MinWidth = 240 };

        // Title
        var titleBorder = new Border();
        titleBorder.Classes.Add("ink-datagrid-popup-title");
        var titleText = new TextBlock { Text = col.Header };
        titleText.Classes.Add("ink-datagrid-popup-title-text");
        titleBorder.Child = titleText;
        stack.Children.Add(titleBorder);

        // Sort section
        if (col.Sortable)
        {
            stack.Children.Add(MakePopupSeparator());

            var current = _sort.FirstOrDefault(s => s.Field == col.FieldName);

            stack.Children.Add(MakePopupItem(
                current?.Direction == SortDirection.Ascending ? "↑  Sort ascending  ✓" : "↑  Sort ascending",
                () => { flyout.Hide(); ApplySort(col.FieldName, SortDirection.Ascending); }));

            stack.Children.Add(MakePopupItem(
                current?.Direction == SortDirection.Descending ? "↓  Sort descending  ✓" : "↓  Sort descending",
                () => { flyout.Hide(); ApplySort(col.FieldName, SortDirection.Descending); }));
        }

        // Filter section
        if (col.FilterEditor is { } editor)
        {
            stack.Children.Add(MakePopupSeparator());

            var filterWrap = new Border();
            filterWrap.Classes.Add("ink-datagrid-popup-filter");
            filterWrap.Child = editor.BuildControl();
            stack.Children.Add(filterWrap);

            stack.Children.Add(MakePopupSeparator());

            var footer = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
            footer.Classes.Add("ink-datagrid-popup-footer");

            var applyBtn = new Button { Content = "Apply" };
            applyBtn.Classes.Add("ink-primary");
            applyBtn.Click += (_, _) =>
            {
                var filter = editor.BuildFilter(col.FieldName);
                if (filter is not null) _activeFilters[col.FieldName] = filter;
                else _activeFilters.Remove(col.FieldName);
                editor.SetFilter(filter);
                flyout.Hide();
                LoadRows();
            };

            var clearBtn = new Button { Content = "Clear" };
            clearBtn.Click += (_, _) =>
            {
                _activeFilters.Remove(col.FieldName);
                editor.SetFilter(null);
                flyout.Hide();
                LoadRows();
            };

            footer.Children.Add(applyBtn);
            footer.Children.Add(clearBtn);
            stack.Children.Add(footer);
        }

        flyout.Content = stack;
        return flyout;
    }

    private static Border MakePopupSeparator()
    {
        var sep = new Border();
        sep.Classes.Add("ink-datagrid-popup-separator");
        return sep;
    }

    private static Button MakePopupItem(string text, Action onClick)
    {
        var btn = new Button
        {
            Content                    = text,
            HorizontalAlignment        = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Left,
        };
        btn.Classes.Add("ink-ghost");
        btn.Classes.Add("ink-datagrid-popup-item");
        btn.Click += (_, _) => onClick();
        return btn;
    }

    // -------------------------------------------------------------------------
    // Sort
    // -------------------------------------------------------------------------

    private void ApplySort(string fieldName, SortDirection direction)
    {
        var existing = _sort.FirstOrDefault(s => s.Field == fieldName);

        if (existing is not null)
        {
            if (existing.Direction == direction)
            {
                _sort.Remove(existing); // same direction clicked again = toggle off
            }
            else
            {
                var idx = _sort.IndexOf(existing);
                _sort[idx] = new SortDescriptor(fieldName, direction); // change direction, preserve priority
            }
        }
        else
        {
            _sort.Add(new SortDescriptor(fieldName, direction)); // new sort, lowest priority
        }

        UpdateAllSortIndicators();
        LoadRows();
    }

    private void UpdateAllSortIndicators()
    {
        foreach (var (fieldName, indicator) in _sortIndicators)
            UpdateSortIndicator(indicator, fieldName);
    }

    private void UpdateSortIndicator(TextBlock indicator, string fieldName)
    {
        var sort = _sort.FirstOrDefault(s => s.Field == fieldName);
        if (sort is null) { indicator.Text = string.Empty; return; }

        var arrow    = sort.Direction == SortDirection.Ascending ? "↑" : "↓";
        var priority = _sort.IndexOf(sort) + 1;
        indicator.Text = _sort.Count > 1 ? $"{arrow}{priority}" : arrow;
    }

    // -------------------------------------------------------------------------
    // Query + data loading
    // -------------------------------------------------------------------------

    private DataQuery BuildQuery() =>
        new(null, [.. _sort], _activeFilters.Values.And(), 1, 25);

    private void LoadRows()
    {
        _cts?.Cancel();
        _cts?.Dispose();

        if (_body is null || Model is null)
        {
            _body?.Children.Clear();
            return;
        }

        _cts = new CancellationTokenSource();
        var ct    = _cts.Token;
        var model = Model;
        var query = BuildQuery();

        _ = Task.Run(async () =>
        {
            var page = await model.ExecuteAsync(query, ct).ConfigureAwait(false);
            if (ct.IsCancellationRequested) return;

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (ct.IsCancellationRequested) return;
                _body.Children.Clear();
                foreach (var item in page.Items)
                    _body.Children.Add(BuildRow(item, model.Columns));
            });
        }, ct);
    }

    // -------------------------------------------------------------------------
    // Row rendering
    // -------------------------------------------------------------------------

    private Border BuildRow(object item, IReadOnlyList<IColumn> columns)
    {
        var row = new Border();
        row.Classes.Add("ink-datagrid-row");

        var grid = BuildColumnGrid(columns);

        for (var i = 0; i < columns.Count; i++)
        {
            var cell = new Border();
            cell.Classes.Add("ink-datagrid-cell");

            var text = new TextBlock { Text = columns[i].GetValue(item)?.ToString() ?? string.Empty };
            text.Classes.Add("ink-datagrid-cell-text");

            cell.Child = text;
            Grid.SetColumn(cell, i);
            grid.Children.Add(cell);
        }

        row.Child = grid;
        return row;
    }

    private static Grid BuildColumnGrid(IReadOnlyList<IColumn> columns)
    {
        var grid = new Grid();
        for (var j = 0; j < columns.Count; j++)
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        return grid;
    }
}
