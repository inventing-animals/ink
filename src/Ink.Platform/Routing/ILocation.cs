using System.Collections.Generic;

namespace Ink.Platform.Routing;

/// <summary>
/// Represents a parsed URL location.
/// </summary>
public interface ILocation
{
    /// <summary>Gets the path component, e.g. <c>/module/purchase-order/detail/123</c>.</summary>
    string Path { get; }

    /// <summary>Gets the path split into segments, e.g. <c>["module", "purchase-order", "detail", "123"]</c>.</summary>
    IReadOnlyList<string> Segments { get; }

    /// <summary>Gets the raw query string without the leading <c>?</c>, or <c>null</c> if absent.</summary>
    string? Query { get; }

    /// <summary>Gets the parsed query string parameters.</summary>
    IReadOnlyDictionary<string, string> QueryParameters { get; }

    /// <summary>Gets the fragment without the leading <c>#</c>, or <c>null</c> if absent.</summary>
    string? Fragment { get; }
}
