using Ink.Platform.Routing;

namespace Ink.Demo;

/// <summary>
/// Application-scoped state shared across all view models.
/// </summary>
public class AppState
{
    public AppState(IRouter router)
    {
        Router = router;
    }

    public IRouter Router { get; }
}
