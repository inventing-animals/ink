using System.Globalization;

namespace Ink.Localization;

/// <summary>
/// Selects the correct CLDR plural form suffix for a given culture and count.
/// Supported suffixes: one, two, few, many, other, zero.
/// </summary>
public static class PluralSelector
{
    /// <summary>Returns the CLDR plural suffix (<c>one</c>, <c>few</c>, <c>many</c>, <c>other</c>, etc.) for <paramref name="count"/> in <paramref name="culture"/>.</summary>
    public static string GetSuffix(CultureInfo culture, long count)
    {
        return culture.TwoLetterISOLanguageName switch
        {
            // Two forms: one / other
            "en" or "de" or "nl" or "sv" or "nb" or "da" or "et"
                or "pt" or "hu" or "ca" or "el" or "he" or "it"
                or "tr" or "fi" or "bg" or "ko" or "id" or "th"
                => count == 1 ? "one" : "other",

            // Two forms: one applies to 0 and 1
            "fr" or "ff" => count is 0 or 1 ? "one" : "other",

            // Polish: one / few / many
            "pl" => count == 1 ? "one"
                : count % 10 is >= 2 and <= 4 && count % 100 is < 12 or > 14 ? "few"
                : "many",

            // Czech / Slovak: one / few / other
            "cs" or "sk" => count == 1 ? "one"
                : count is >= 2 and <= 4 ? "few"
                : "other",

            // Russian / Ukrainian: one / few / many
            "ru" or "uk" => count % 10 == 1 && count % 100 != 11 ? "one"
                : count % 10 is >= 2 and <= 4 && count % 100 is < 12 or > 14 ? "few"
                : "many",

            // Arabic: zero / one / two / few / many / other
            "ar" => count == 0 ? "zero"
                : count == 1 ? "one"
                : count == 2 ? "two"
                : count % 100 is >= 3 and <= 10 ? "few"
                : count % 100 is >= 11 and <= 99 ? "many"
                : "other",

            // Japanese / Chinese: no plural distinctions
            "ja" or "zh" or "vi" or "ms" => "other",

            // Safe fallback for unlisted languages
            _ => count == 1 ? "one" : "other",
        };
    }
}
