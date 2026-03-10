using System;

namespace Ink.Platform.Routing;

/// <summary>
/// Manages application navigation state.
/// </summary>
public interface IRouter
{
    /// <summary>Gets the current location.</summary>
    ILocation Current { get; }

    /// <summary>Navigates to the specified path, pushing a new entry onto the history stack.</summary>
    void Navigate(string path);

    /// <summary>Navigates to the specified path, replacing the current history entry.</summary>
    void Replace(string path);

    /// <summary>Moves back one entry in the history stack.</summary>
    void Back();

    /// <summary>Moves forward one entry in the history stack.</summary>
    void Forward();

    /// <summary>Raised when the current location changes.</summary>
    event EventHandler<ILocation>? LocationChanged;
}
