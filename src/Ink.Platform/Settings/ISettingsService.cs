using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Metadata;

namespace Ink.Platform.Settings;

/// <summary>
/// Provides persistent key-value storage for application settings.
/// </summary>
public interface ISettingsService
{
    /// <summary>Gets the value associated with the specified key, or <c>default</c> if not found.</summary>
    /// <typeparam name="T">The type to deserialize the value as.</typeparam>
    /// <param name="key">The settings key.</param>
    /// <param name="typeInfo">The JSON type info for <typeparamref name="T"/>.</param>
    /// <returns>The deserialized value, or <c>default</c> if the key does not exist.</returns>
    T? Get<T>(string key, JsonTypeInfo<T> typeInfo);

    /// <summary>Sets the value for the specified key.</summary>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    /// <param name="key">The settings key.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="typeInfo">The JSON type info for <typeparamref name="T"/>.</param>
    void Set<T>(string key, T value, JsonTypeInfo<T> typeInfo);

    /// <summary>Removes the value associated with the specified key.</summary>
    /// <param name="key">The settings key.</param>
    void Remove(string key);

    /// <summary>Returns <c>true</c> if a value exists for the specified key.</summary>
    /// <param name="key">The settings key.</param>
    /// <returns><c>true</c> if the key exists; otherwise <c>false</c>.</returns>
    bool Contains(string key);
}
