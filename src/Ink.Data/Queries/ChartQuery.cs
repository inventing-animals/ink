using System;
using System.Collections.Generic;

namespace Ink.Data.Queries;

/// <summary>
/// Describes a time-windowed aggregation query for chart data.
/// Shares the same <see cref="FilterNode"/> model as <see cref="DataGridQuery"/>
/// so the same filter UI can drive both grids and charts.
/// </summary>
/// <param name="From">Start of the time window (inclusive).</param>
/// <param name="To">End of the time window (inclusive).</param>
/// <param name="Granularity">How to bucket data points within the window.</param>
/// <param name="Filter">Optional filter applied before aggregation.</param>
/// <param name="Fields">Series field names to include, or <c>null</c> for all.</param>
public sealed record ChartQuery(
    DateTimeOffset From,
    DateTimeOffset To,
    ChartGranularity Granularity,
    FilterNode? Filter = null,
    IReadOnlyList<string>? Fields = null);
