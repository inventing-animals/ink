using System.Collections.Generic;

namespace Ink.Demo.ViewModels;

public class RadioButtonViewModel : DemoPageViewModel
{
    public override string Title => "RadioButton";
    public override IReadOnlyList<string> Subpages { get; } = ["Ink", "Avalonia"];
}
