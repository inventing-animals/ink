using System.Collections.Generic;
using System.Globalization;
using System.Resources;

namespace Ink.Localization;

/// <summary>
/// <see cref="ILocalizationService"/> implementation backed by a list of <see cref="ResourceManager"/> instances.
/// Managers are searched in order; the first non-null value wins.
/// </summary>
public sealed class LocalizationService : ILocalizationService
{
    private readonly IReadOnlyList<ResourceManager> _managers;

    /// <summary>Initializes a new instance with the given resource managers, searched in order.</summary>
    public LocalizationService(IReadOnlyList<ResourceManager> managers)
    {
        _managers = managers;
    }

    /// <inheritdoc/>
    public string Get(string key)
    {
        foreach (var manager in _managers)
        {
            var value = manager.GetString(key, CultureInfo.CurrentUICulture);
            if (value is not null)
                return value;
        }

        return key;
    }

    /// <inheritdoc/>
    public string GetPlural(string key, long count, params object[] args)
    {
        var suffix = PluralSelector.GetSuffix(CultureInfo.CurrentUICulture, count);

        // Try exact suffix, then fall back to "other" (required by CLDR for all languages)
        var template = TryGet($"{key}.{suffix}") ?? TryGet($"{key}.other") ?? key;

        return args.Length > 0 ? string.Format(CultureInfo.CurrentUICulture, template, args) : template;
    }

    private string? TryGet(string key)
    {
        foreach (var manager in _managers)
        {
            var value = manager.GetString(key, CultureInfo.CurrentUICulture);
            if (value is not null)
                return value;
        }

        return null;
    }
}
