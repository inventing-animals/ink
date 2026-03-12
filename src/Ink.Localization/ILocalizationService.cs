namespace Ink.Localization;

/// <summary>
/// Client-side localization service. Uses <see cref="System.Globalization.CultureInfo.CurrentUICulture"/> implicitly.
/// </summary>
public interface ILocalizationService
{
    /// <summary>Gets the localized string for <paramref name="key"/>.</summary>
    string Get(string key);

    /// <summary>Gets the plural-form localized string for <paramref name="key"/>, formatted with <paramref name="args"/>.</summary>
    string GetPlural(string key, long count, params object[] args);
}
