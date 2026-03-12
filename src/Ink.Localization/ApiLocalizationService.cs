using System.Collections.Generic;
using System.Globalization;
using System.Resources;

namespace Ink.Localization;

/// <summary>
/// <see cref="IApiLocalizationService"/> implementation backed by a list of <see cref="ResourceManager"/> instances.
/// Managers are searched in order; the first non-null value wins.
/// </summary>
public sealed class ApiLocalizationService : IApiLocalizationService
{
    private readonly IReadOnlyList<ResourceManager> _managers;

    /// <summary>Initializes a new instance with the given resource managers, searched in order.</summary>
    public ApiLocalizationService(IReadOnlyList<ResourceManager> managers)
    {
        _managers = managers;
    }

    /// <inheritdoc/>
    public string Get(string key, CultureInfo culture)
    {
        foreach (var manager in _managers)
        {
            var value = manager.GetString(key, culture);
            if (value is not null)
                return value;
        }

        return key;
    }

    /// <inheritdoc/>
    public string GetPlural(string key, long count, CultureInfo culture, params object[] args)
    {
        var suffix = PluralSelector.GetSuffix(culture, count);
        var template = TryGet($"{key}.{suffix}", culture)
            ?? TryGet($"{key}.other", culture)
            ?? key;

        return args.Length > 0 ? string.Format(culture, template, args) : template;
    }

    private string? TryGet(string key, CultureInfo culture)
    {
        foreach (var manager in _managers)
        {
            var value = manager.GetString(key, culture);
            if (value is not null)
                return value;
        }

        return null;
    }
}
