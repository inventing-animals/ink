namespace Ink.Data.Queries;

/// <summary>Time granularity for chart data aggregation.</summary>
public enum ChartGranularity
{
    /// <summary>Aggregate by minute.</summary>
    Minute,

    /// <summary>Aggregate by hour.</summary>
    Hour,

    /// <summary>Aggregate by day.</summary>
    Day,

    /// <summary>Aggregate by week.</summary>
    Week,

    /// <summary>Aggregate by month.</summary>
    Month,

    /// <summary>Aggregate by year.</summary>
    Year,
}
