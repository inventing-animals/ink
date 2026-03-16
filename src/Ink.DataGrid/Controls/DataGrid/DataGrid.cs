using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Ink.Data.Columns;
using Ink.Data.Queries;

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
        BuildHeader();
        LoadRows();
    }

    private void BuildHeader()
    {
        if (_headerPanel is null)
            return;

        _headerPanel.Children.Clear();

        var columns = Model?.Columns;
        if (columns is null || columns.Count == 0)
            return;

        var grid = BuildColumnGrid(columns);

        for (var i = 0; i < columns.Count; i++)
        {
            var cell = new Border();
            cell.Classes.Add("ink-datagrid-header-cell");

            var text = new TextBlock { Text = columns[i].Header };
            text.Classes.Add("ink-datagrid-header-text");

            cell.Child = text;
            Grid.SetColumn(cell, i);
            grid.Children.Add(cell);
        }

        _headerPanel.Children.Add(grid);
    }

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
        var ct = _cts.Token;
        var model = Model;

        _ = Task.Run(async () =>
        {
            var page = await model.ExecuteAsync(DataQuery.Default, ct).ConfigureAwait(false);
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
