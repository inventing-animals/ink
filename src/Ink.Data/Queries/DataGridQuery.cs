using System.Collections.Generic;

namespace Ink.Data.Queries;

/// <summary>
/// Describes a complete data grid query including visible columns, sort, filter, and pagination.
/// This record is the single contract between the client grid and any data source or API endpoint.
/// </summary>
/// <param name="Columns">
/// The field names of columns the client wants to receive, or <c>null</c> to request all registered columns.
/// Sources and translators use this list for projection and security validation -
/// fields not in the registered column set are rejected regardless of this value.
/// </param>
/// <param name="Sort">Ordered list of sort criteria. Earlier entries take precedence.</param>
/// <param name="Filter">The root filter node, or <c>null</c> for no filtering.</param>
/// <param name="Page">One-based page number.</param>
/// <param name="PageSize">Number of items per page.</param>
public sealed record DataGridQuery(
    IReadOnlyList<string>? Columns,
    IReadOnlyList<SortDescriptor> Sort,
    FilterNode? Filter,
    int Page,
    int PageSize)
{
    /// <summary>Gets a default query for page 1 with 25 items, no filter, no sort.</summary>
    public static DataGridQuery Default => new(null, [], null, 1, 25);
}
