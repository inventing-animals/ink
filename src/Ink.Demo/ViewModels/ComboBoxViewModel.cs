using System.Collections.Generic;

namespace Ink.Demo.ViewModels;

public class ComboBoxViewModel : DemoPageViewModel
{
    public override string Title => "ComboBox";
    public override IReadOnlyList<string> Subpages { get; } = ["Ink", "Avalonia"];
}
