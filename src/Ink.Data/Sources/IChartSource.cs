using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ink.Data.Queries;
using Ink.Data.Results;

namespace Ink.Data.Sources;

/// <summary>
/// Provides aggregated time-series data for charts.
/// The source is responsible for bucketing raw data according to
/// the requested <see cref="ChartQuery.Granularity"/> and <see cref="ChartQuery.Filter"/>.
/// </summary>
public interface IChartSource
{
    /// <summary>Executes the chart query and returns aggregated series.</summary>
    /// <param name="query">The time window, granularity, and optional filter.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>One <see cref="ChartSeries"/> per requested field.</returns>
    Task<IReadOnlyList<ChartSeries>> QueryAsync(ChartQuery query, CancellationToken ct = default);

    /// <summary>
    /// Raised when the underlying data has changed and the chart should re-query.
    /// Default implementation is a no-op.
    /// </summary>
    event Action? Invalidated
    {
        add { }
        remove { }
    }
}
