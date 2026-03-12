using Ink.Platform.Routing;
using Ink.UI.Platform;

namespace Ink.Demo;

/// <summary>
/// Application-scoped state shared across all view models.
/// </summary>
public class AppState(IRouter router, IWindowService windows)
{
    public IRouter        Router  { get; } = router;
    public IWindowService Windows { get; } = windows;
}
