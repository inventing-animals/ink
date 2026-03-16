using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ink.Data.Queries;
using Ink.Data.Results;

namespace Ink.Data.Sources;

/// <summary>
/// An in-memory <see cref="IDataSource{T}"/> backed by a fixed list.
/// Suitable for demos, tests, and small datasets that live entirely in memory.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public sealed class ListDataSource<T> : IDataSource<T>
{
    private readonly IReadOnlyList<T> _items;

    public ListDataSource(IReadOnlyList<T> items) => _items = items;

    public event Action? Invalidated { add { } remove { } }

    public Task<DataPage<T>> QueryAsync(DataQuery query, CancellationToken ct = default)
    {
        var skip = (query.Page - 1) * query.PageSize;
        var page = _items.Skip(skip).Take(query.PageSize).ToList();
        return Task.FromResult(new DataPage<T>(page, _items.Count));
    }
}
