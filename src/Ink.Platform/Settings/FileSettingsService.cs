using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Ink.Platform.Settings;

/// <summary>
/// Stores settings as a JSON file on the local file system.
/// Suitable for desktop and mobile platforms.
/// </summary>
public class FileSettingsService : ISettingsService
{
    private readonly string _filePath;
    private readonly Dictionary<string, JsonElement> _store;

    /// <summary>Initializes a new instance of the <see cref="FileSettingsService"/> class.</summary>
    /// <param name="filePath">The path to the JSON settings file.</param>
    public FileSettingsService(string filePath)
    {
        _filePath = filePath;
        _store = Load();
    }

    /// <inheritdoc/>
    public T? Get<T>(string key, JsonTypeInfo<T> typeInfo)
    {
        if (_store.TryGetValue(key, out var element))
            return element.Deserialize(typeInfo);
        return default;
    }

    /// <inheritdoc/>
    public void Set<T>(string key, T value, JsonTypeInfo<T> typeInfo)
    {
        _store[key] = JsonSerializer.SerializeToElement(value, typeInfo);
        Save();
    }

    /// <inheritdoc/>
    public void Remove(string key)
    {
        if (_store.Remove(key))
            Save();
    }

    /// <inheritdoc/>
    public bool Contains(string key) => _store.ContainsKey(key);

    private Dictionary<string, JsonElement> Load()
    {
        if (!File.Exists(_filePath))
            return [];

        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize(json, SettingsStoreJsonContext.Default.DictionaryStringJsonElement) ?? [];
    }

    private void Save()
    {
        var dir = Path.GetDirectoryName(_filePath);
        if (dir is not null)
            Directory.CreateDirectory(dir);

        File.WriteAllText(_filePath, JsonSerializer.Serialize(_store, SettingsStoreJsonContext.Default.DictionaryStringJsonElement));
    }
}
