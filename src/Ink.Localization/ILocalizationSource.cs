using System.Globalization;

namespace Ink.Localization;

/// <summary>
/// Abstraction over a single source of localized strings.
/// Multiple sources can be chained; the first non-null value wins.
/// </summary>
public interface ILocalizationSource
{
    /// <summary>Returns the localized string for <paramref name="key"/> in <paramref name="culture"/>, or <see langword="null"/> if not found.</summary>
    string? TryGet(string key, CultureInfo culture);
}
