using System;
using System.Collections.Generic;

namespace Ink.Platform.Routing;

/// <summary>
/// An in-memory router that tracks navigation state without any platform integration.
/// Suitable for desktop, mobile, and unit testing scenarios.
/// </summary>
public class InMemoryRouter : IRouter
{
    private readonly List<ILocation> _history = [];
    private int _index = -1;

    /// <summary>Initializes a new instance of the <see cref="InMemoryRouter"/> class.</summary>
    /// <param name="initialPath">The initial path to start at.</param>
    public InMemoryRouter(string initialPath = "/")
    {
        Push(Location.Parse(initialPath));
    }

    /// <inheritdoc/>
    public event EventHandler<ILocation>? LocationChanged;

    /// <inheritdoc/>
    public ILocation Current => _history[_index];

    /// <inheritdoc/>
    public void Navigate(string path)
    {
        if (_index < _history.Count - 1)
        {
            _history.RemoveRange(_index + 1, _history.Count - _index - 1);
        }

        Push(Location.Parse(path));
        OnLocationChanged();
    }

    /// <inheritdoc/>
    public void Replace(string path)
    {
        _history[_index] = Location.Parse(path);
        OnLocationChanged();
    }

    /// <inheritdoc/>
    public void Back()
    {
        if (_index > 0)
        {
            _index--;
            OnLocationChanged();
        }
    }

    /// <inheritdoc/>
    public void Forward()
    {
        if (_index < _history.Count - 1)
        {
            _index++;
            OnLocationChanged();
        }
    }

    private void Push(ILocation location)
    {
        _history.Add(location);
        _index = _history.Count - 1;
    }

    private void OnLocationChanged() =>
        LocationChanged?.Invoke(this, Current);
}
