using System.Collections.Generic;

namespace Ink.Demo.ViewModels;

public class TextBoxViewModel : DemoPageViewModel
{
    public override string Title => "TextBox";
    public override IReadOnlyList<string> Subpages { get; } = ["Ink", "Avalonia"];
}
