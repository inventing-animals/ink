namespace Ink.Localization;

/// <summary>
/// No-op <see cref="ILocalizationService"/> used before the service is initialized.
/// Returns the key itself so missing strings are visible rather than silent.
/// </summary>
public sealed class NullLocalizationService : ILocalizationService
{
    /// <summary>Shared singleton instance.</summary>
    public static readonly NullLocalizationService Instance = new();

    /// <inheritdoc/>
    public string Get(string key) => key;

    /// <inheritdoc/>
    public string GetPlural(string key, long count, params object[] args) => key;
}
