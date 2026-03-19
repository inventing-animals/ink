using System.Collections.Generic;

namespace Ink.Demo.ViewModels;

public class ListBoxViewModel : DemoPageViewModel
{
    public override string Title => "ListBox";
    public override IReadOnlyList<string> Subpages { get; } = ["Ink", "Avalonia"];
}
