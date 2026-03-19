using System.Collections.Generic;

namespace Ink.Demo.ViewModels;

public class ScrollViewViewModel : DemoPageViewModel
{
    public override string Title => "ScrollViewer";
    public override IReadOnlyList<string> Subpages { get; } = ["Ink", "Avalonia"];
}
