using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Ink.Platform.Settings;

namespace Ink.Platform.Browser.Settings;

/// <summary>
/// Stores settings in the browser's <c>localStorage</c>.
/// Suitable for WASM/browser platforms.
/// </summary>
public partial class LocalStorageSettingsService : ISettingsService
{
    /// <inheritdoc/>
    public T? Get<T>(string key, JsonTypeInfo<T> typeInfo)
    {
        var value = GetItem(key);
        if (value is null) return default;
        return JsonSerializer.Deserialize(value, typeInfo);
    }

    /// <inheritdoc/>
    public void Set<T>(string key, T value, JsonTypeInfo<T> typeInfo) =>
        SetItem(key, JsonSerializer.Serialize(value, typeInfo));

    /// <inheritdoc/>
    public void Remove(string key) => RemoveItem(key);

    /// <inheritdoc/>
    public bool Contains(string key) => GetItem(key) is not null;

    [JSImport("globalThis.localStorage.getItem")]
    private static partial string? GetItem(string key);

    [JSImport("globalThis.localStorage.setItem")]
    private static partial void SetItem(string key, string value);

    [JSImport("globalThis.localStorage.removeItem")]
    private static partial void RemoveItem(string key);
}
