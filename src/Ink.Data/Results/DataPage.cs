using System.Collections.Generic;

namespace Ink.Data.Results;

/// <summary>
/// A single page of query results together with the total count across all pages.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
/// <param name="Items">The items on this page.</param>
/// <param name="TotalCount">Total number of items matching the query, used for pagination UI.</param>
public sealed record DataPage<T>(IReadOnlyList<T> Items, int TotalCount);
