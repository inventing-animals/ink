namespace Ink.Data.Results;

/// <summary>A single data point in a chart series.</summary>
/// <param name="X">The X-axis value. Typically a <see cref="System.DateTimeOffset"/> for time series or a string label.</param>
/// <param name="Y">The Y-axis value.</param>
public sealed record ChartPoint(object X, double Y);
