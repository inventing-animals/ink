using System.Collections.Generic;

namespace Ink.Demo.ViewModels;

public class TypographyViewModel : DemoPageViewModel
{
    public override string Title => "Typography";
    public override IReadOnlyList<string> Subpages { get; } = ["Label", "TextBlock", "SelectableTextBlock"];
}
