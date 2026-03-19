using System.Collections.Generic;

namespace Ink.Demo.ViewModels;

public class PopupsViewModel : DemoPageViewModel
{
    public override string Title => "Popups";
    public override IReadOnlyList<string> Subpages { get; } = ["Ink", "Avalonia"];
}
