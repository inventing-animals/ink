using System.Collections.Generic;

namespace Ink.Data.Results;

/// <summary>A named series of aggregated data points for charting.</summary>
/// <param name="Label">The display label for this series.</param>
/// <param name="Points">The data points ordered by X value.</param>
public sealed record ChartSeries(string Label, IReadOnlyList<ChartPoint> Points);
