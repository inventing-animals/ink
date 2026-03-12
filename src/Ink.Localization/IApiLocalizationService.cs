using System.Globalization;

namespace Ink.Localization;

/// <summary>
/// Server-side localization service. Culture is supplied explicitly per request.
/// </summary>
public interface IApiLocalizationService
{
    /// <summary>Gets the localized string for <paramref name="key"/> in <paramref name="culture"/>.</summary>
    string Get(string key, CultureInfo culture);

    /// <summary>Gets the plural-form localized string for <paramref name="key"/> in <paramref name="culture"/>, formatted with <paramref name="args"/>.</summary>
    string GetPlural(string key, long count, CultureInfo culture, params object[] args);
}
