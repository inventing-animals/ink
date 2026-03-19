using System.Collections.Generic;

namespace Ink.Demo.ViewModels;

public class ButtonsViewModel : DemoPageViewModel
{
    public override string Title => "Buttons";
    public override IReadOnlyList<string> Subpages { get; } = ["Ink", "Avalonia"];
}
