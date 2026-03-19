using System.Collections.Generic;

namespace Ink.Demo.ViewModels;

public class ProgressBarViewModel : DemoPageViewModel
{
    public override string Title => "ProgressBar";
    public override IReadOnlyList<string> Subpages { get; } = ["Ink", "Avalonia"];
}
