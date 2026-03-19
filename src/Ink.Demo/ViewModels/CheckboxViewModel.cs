using System.Collections.Generic;

namespace Ink.Demo.ViewModels;

public class CheckboxViewModel : DemoPageViewModel
{
    public override string Title => "Checkbox";
    public override IReadOnlyList<string> Subpages { get; } = ["Ink", "Avalonia"];
}
