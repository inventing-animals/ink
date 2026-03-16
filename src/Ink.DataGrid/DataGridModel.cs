using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ink.Data.Columns;
using Ink.Data.Queries;
using Ink.Data.Results;
using Ink.Data.Sources;

namespace Ink.DataGrid;

/// <summary>
/// Base class for view models that drive a DataGrid.
/// Inherit from <see cref="DataGridModel{T}"/> to connect a typed <see cref="IDataSource{T}"/>.
/// </summary>
public abstract class DataGridModel
{
    /// <summary>Gets the column definitions for this grid.</summary>
    public IReadOnlyList<IColumn> Columns { get; protected set; } = [];

    /// <summary>Executes a query and returns a page of untyped items.</summary>
    internal abstract Task<DataPage<object>> ExecuteAsync(DataQuery query, CancellationToken ct);
}

/// <summary>
/// A <see cref="DataGridModel"/> backed by an <see cref="IDataSource{T}"/>.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public abstract class DataGridModel<T> : DataGridModel
{
    /// <summary>Gets or sets the data source that provides items to this grid.</summary>
    protected IDataSource<T> Source { get; set; } = new ListDataSource<T>([]);

    internal override async Task<DataPage<object>> ExecuteAsync(DataQuery query, CancellationToken ct)
    {
        var page = await Source.QueryAsync(query, ct).ConfigureAwait(false);
        var items = new List<object>(page.Items.Count);
        foreach (var item in page.Items)
            items.Add(item!);
        return new DataPage<object>(items, page.TotalCount);
    }
}
