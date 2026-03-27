using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;

namespace Ink.Localization;

/// <summary>
/// <see cref="IApiLocalizationService"/> implementation backed by a list of <see cref="ILocalizationSource"/> instances.
/// Sources are searched in order; the first non-null value wins.
/// </summary>
public sealed class ApiLocalizationService : IApiLocalizationService
{
    private readonly IReadOnlyList<ILocalizationSource> _sources;

    /// <summary>Initializes a new instance with the given sources, searched in order.</summary>
    public ApiLocalizationService(IReadOnlyList<ILocalizationSource> sources)
    {
        _sources = sources;
    }

    /// <summary>Initializes a new instance backed by <see cref="ResourceManager"/> instances (backward-compatible overload).</summary>
    public ApiLocalizationService(IReadOnlyList<ResourceManager> managers)
        : this(managers.Select(m => (ILocalizationSource)new ResourceManagerLocalizationSource(m)).ToList())
    {
    }

    /// <inheritdoc/>
    public string Get(string key, CultureInfo culture)
    {
        foreach (var source in _sources)
        {
            var value = source.TryGet(key, culture);
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
        foreach (var source in _sources)
        {
            var value = source.TryGet(key, culture);
            if (value is not null)
                return value;
        }

        return null;
    }
}
