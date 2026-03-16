using System;
using System.Globalization;
using System.Text.Json;

namespace Ink.Data.EFCore.Internal;

/// <summary>
/// Coerces filter values (which may arrive as boxed primitives or <see cref="JsonElement"/>
/// after JSON deserialization) to the target member type.
/// </summary>
internal static class TypeCoercion
{
    /// <summary>
    /// Converts <paramref name="value"/> to <paramref name="targetType"/>,
    /// stripping nullable wrappers before conversion.
    /// </summary>
    public static object? Coerce(object? value, Type targetType)
    {
        if (value is null)
            return null;

        var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (underlying.IsInstanceOfType(value))
            return value;

        if (value is JsonElement elem)
            return CoerceJsonElement(elem, underlying);

        if (underlying == typeof(Guid) && value is string s1)
            return Guid.Parse(s1);

        if (underlying == typeof(DateTime) && value is string s2)
            return DateTime.Parse(s2, null, DateTimeStyles.RoundtripKind);

        if (underlying == typeof(DateTimeOffset) && value is string s3)
            return DateTimeOffset.Parse(s3, null, DateTimeStyles.RoundtripKind);

        if (underlying == typeof(DateOnly) && value is string s4)
            return DateOnly.Parse(s4, CultureInfo.InvariantCulture);

        if (underlying == typeof(TimeOnly) && value is string s5)
            return TimeOnly.Parse(s5, CultureInfo.InvariantCulture);

        if (underlying.IsEnum && value is string enumStr)
            return Enum.Parse(underlying, enumStr, ignoreCase: true);

        if (underlying.IsEnum)
            return Enum.ToObject(underlying, Convert.ChangeType(value, Enum.GetUnderlyingType(underlying), CultureInfo.InvariantCulture));

        return Convert.ChangeType(value, underlying, CultureInfo.InvariantCulture);
    }

    private static object? CoerceJsonElement(JsonElement elem, Type targetType) =>
        targetType switch
        {
            var t when t == typeof(string) => elem.GetString(),
            var t when t == typeof(bool) => elem.GetBoolean(),
            var t when t == typeof(int) => elem.GetInt32(),
            var t when t == typeof(long) => elem.GetInt64(),
            var t when t == typeof(double) => elem.GetDouble(),
            var t when t == typeof(float) => (float)elem.GetDouble(),
            var t when t == typeof(decimal) => elem.GetDecimal(),
            var t when t == typeof(Guid) => elem.GetGuid(),
            var t when t == typeof(DateTime) => elem.GetDateTime(),
            var t when t == typeof(DateTimeOffset) => elem.GetDateTimeOffset(),
            var t when t == typeof(DateOnly) => DateOnly.FromDateTime(elem.GetDateTime()),
            _ => Convert.ChangeType(elem.ToString(), targetType, CultureInfo.InvariantCulture),
        };
}
