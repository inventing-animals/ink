using System.Globalization;
using System.Resources;

namespace Ink.Localization;

/// <summary>
/// <see cref="ILocalizationSource"/> backed by a <see cref="ResourceManager"/> (ResX / satellite assemblies).
/// </summary>
public sealed class ResourceManagerLocalizationSource : ILocalizationSource
{
    private readonly ResourceManager _manager;

    /// <summary>Initializes a new instance wrapping <paramref name="manager"/>.</summary>
    public ResourceManagerLocalizationSource(ResourceManager manager) => _manager = manager;

    /// <inheritdoc/>
    public string? TryGet(string key, CultureInfo culture) => _manager.GetString(key, culture);
}
