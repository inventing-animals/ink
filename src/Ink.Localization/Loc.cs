namespace Ink.Localization;

/// <summary>
/// Static ambient accessor for the current <see cref="ILocalizationService"/>.
/// Use from ViewModels: <c>Loc.Get("key")</c>, <c>Loc.Plural("key", count, count)</c>.
/// </summary>
public static class Loc
{
    private static ILocalizationService _service = NullLocalizationService.Instance;

    /// <summary>Replaces the active service. Call once at startup from your bootstrapper.</summary>
    public static void Initialize(ILocalizationService service) => _service = service;

    /// <summary>Gets the localized string for <paramref name="key"/>.</summary>
    public static string Get(string key) => _service.Get(key);

    /// <summary>Gets the plural-form localized string for <paramref name="key"/>.</summary>
    public static string Plural(string key, long count, params object[] args) =>
        _service.GetPlural(key, count, args);
}
