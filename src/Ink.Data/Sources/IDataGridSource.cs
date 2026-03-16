using System;
using System.Threading;
using System.Threading.Tasks;
using Ink.Data.Queries;
using Ink.Data.Results;

namespace Ink.Data.Sources;

/// <summary>
/// Provides paginated, filtered, and sorted data for a data grid.
/// Implement this interface to connect a grid to any data source -
/// in-memory collections, HTTP APIs, databases, or real-time streams.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public interface IDataGridSource<T>
{
    /// <summary>Executes the query and returns a single page of results.</summary>
    /// <param name="query">The query describing sort, filter, pagination, and requested columns.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A page of matching items and the total count across all pages.</returns>
    Task<DataPage<T>> QueryAsync(DataGridQuery query, CancellationToken ct = default);

    /// <summary>
    /// Raised when the underlying data has changed and the grid should re-query.
    /// The default implementation is a no-op; sources that support real-time updates
    /// should override this with a backing field and fire it when data changes.
    /// </summary>
    event Action? Invalidated
    {
        add { }
        remove { }
    }
}
