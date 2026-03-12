namespace Ink.UI.Platform;

public sealed class WindowOptions
{
    public string? Title  { get; init; }
    public double? Width  { get; init; }
    public double? Height { get; init; }

    /// <summary>
    /// Router path for platforms that open a new app instance (e.g. browser tabs).
    /// Desktop and mobile ignore this — they use the <c>Func&lt;Control&gt;</c> content factory.
    /// </summary>
    public string? Url { get; init; }
}
