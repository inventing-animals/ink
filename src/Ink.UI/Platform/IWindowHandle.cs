using System.Threading.Tasks;

namespace Ink.UI.Platform;

/// <summary>
/// Represents an open secondary UI surface returned by <see cref="IWindowService.OpenAsync"/>.
/// </summary>
public interface IWindowHandle
{
    /// <summary>Closes the surface immediately.</summary>
    void Close();

    /// <summary>Returns a task that completes when the surface is closed by any means.</summary>
    Task WaitForCloseAsync();
}
