using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Ink.Localization;

/// <summary>
/// <see cref="ILocalizationSource"/> that reads GNU gettext <c>.po</c> files from disk.
/// </summary>
/// <remarks>
/// Expected directory layout:
/// <code>
/// basePath/
///   en/
///     common.po
///     countries.po
///   cs/
///     common.po
///     countries.po
/// </code>
/// All <c>.po</c> files within a language directory are merged.
/// When a key is not found in the requested language, falls back to the fallback language (default: <c>"en"</c>).
/// All files are read once at construction time and cached in memory.
/// </remarks>
public sealed class PoFileLocalizationSource : ILocalizationSource
{
    // language code → key → value
    private readonly Dictionary<string, Dictionary<string, string>> _cache;
    private readonly string _fallbackLanguage;

    /// <summary>
    /// Initializes the source by reading all <c>.po</c> files under <paramref name="basePath"/>.
    /// </summary>
    /// <param name="basePath">Directory containing per-language subdirectories.</param>
    /// <param name="fallbackLanguage">Language code to fall back to when a key is missing. Defaults to <c>"en"</c>.</param>
    public PoFileLocalizationSource(string basePath, string fallbackLanguage = "en")
    {
        _fallbackLanguage = fallbackLanguage;
        _cache = Load(basePath);
    }

    private static Dictionary<string, Dictionary<string, string>> Load(string basePath)
    {
        var result = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        if (!Directory.Exists(basePath))
            return result;

        foreach (var langDir in Directory.GetDirectories(basePath))
        {
            if (Path.GetFileName(langDir) is not { Length: > 0 } language)
                continue;

            var entries = new Dictionary<string, string>(StringComparer.Ordinal);

            foreach (var poFile in Directory.GetFiles(langDir, "*.po"))
                ParseInto(poFile, entries);

            result[language] = entries;
        }

        return result;
    }

    private static void ParseInto(string path, Dictionary<string, string> entries)
    {
        string? currentId = null;

        foreach (var rawLine in File.ReadLines(path))
        {
            var line = rawLine.Trim();

            if (line.Length == 0 || line[0] == '#')
            {
                currentId = null;
                continue;
            }

            if (line.StartsWith("msgid \"", StringComparison.Ordinal))
            {
                currentId = Unescape(line[7..^1]);
            }
            else if (line.StartsWith("msgstr \"", StringComparison.Ordinal) && currentId is { Length: > 0 })
            {
                var value = Unescape(line[8..^1]);
                if (value.Length > 0)
                    entries[currentId] = value;

                currentId = null;
            }
        }
    }

    private static string Unescape(string s) =>
        s.Replace("\\n", "\n", StringComparison.Ordinal)
         .Replace("\\t", "\t", StringComparison.Ordinal)
         .Replace("\\\"", "\"", StringComparison.Ordinal)
         .Replace("\\\\", "\\", StringComparison.Ordinal);

    /// <inheritdoc/>
    public string? TryGet(string key, CultureInfo culture)
    {
        // Try full BCP 47 name first (e.g. "cs-CZ"), then language only (e.g. "cs"), then fallback.
        return TryGetFromLanguage(key, culture.Name)
            ?? TryGetFromLanguage(key, culture.TwoLetterISOLanguageName)
            ?? TryGetFromLanguage(key, _fallbackLanguage);
    }

    private string? TryGetFromLanguage(string key, string language)
    {
        if (_cache.TryGetValue(language, out var entries) && entries.TryGetValue(key, out var value))
            return value;
        return null;
    }
}
