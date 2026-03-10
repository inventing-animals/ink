using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ink.Platform.Settings;

[JsonSerializable(typeof(Dictionary<string, JsonElement>))]
internal partial class SettingsStoreJsonContext : JsonSerializerContext
{
}
