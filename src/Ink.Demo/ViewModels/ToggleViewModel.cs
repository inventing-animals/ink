using System.Collections.Generic;

namespace Ink.Demo.ViewModels;

public class ToggleViewModel : DemoPageViewModel
{
    public override string Title => "Toggle";
    public override IReadOnlyList<string> Subpages { get; } = ["Ink", "Avalonia"];
}
