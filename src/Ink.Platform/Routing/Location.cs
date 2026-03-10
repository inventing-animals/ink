using System;
using System.Collections.Generic;
using System.Web;

namespace Ink.Platform.Routing;

/// <summary>
/// A parsed representation of a URL location.
/// </summary>
public sealed class Location : ILocation
{
    /// <summary>Initializes a new instance of the <see cref="Location"/> class.</summary>
    public Location(string path, string? query = null, string? fragment = null)
    {
        Path = path;
        Segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        Query = string.IsNullOrEmpty(query) ? null : query;
        Fragment = string.IsNullOrEmpty(fragment) ? null : fragment;
        QueryParameters = ParseQuery(Query);
    }

    /// <inheritdoc/>
    public string Path { get; }

    /// <inheritdoc/>
    public IReadOnlyList<string> Segments { get; }

    /// <inheritdoc/>
    public string? Query { get; }

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, string> QueryParameters { get; }

    /// <inheritdoc/>
    public string? Fragment { get; }

    /// <summary>Parses a full URL string into a <see cref="Location"/>.</summary>
    /// <param name="url">The URL to parse.</param>
    /// <returns>A <see cref="Location"/> representing the parsed URL.</returns>
    public static Location Parse(string url)
    {
        var uri = new Uri(url, UriKind.RelativeOrAbsolute);

        if (!uri.IsAbsoluteUri)
        {
            uri = new Uri(new Uri("http://localhost"), url);
        }

        var query = string.IsNullOrEmpty(uri.Query) ? null : uri.Query.TrimStart('?');
        var fragment = string.IsNullOrEmpty(uri.Fragment) ? null : uri.Fragment.TrimStart('#');

        return new Location(uri.AbsolutePath, query, fragment);
    }

    private static IReadOnlyDictionary<string, string> ParseQuery(string? query)
    {
        if (string.IsNullOrEmpty(query))
        {
            return new Dictionary<string, string>();
        }

        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var parsed = HttpUtility.ParseQueryString(query);

        foreach (string? key in parsed)
        {
            if (key is not null)
            {
                result[key] = parsed[key] ?? string.Empty;
            }
        }

        return result;
    }
}
