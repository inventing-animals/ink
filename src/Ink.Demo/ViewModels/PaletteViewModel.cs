using System.Collections.Generic;

namespace Ink.Demo.ViewModels;

public class PaletteViewModel : DemoPageViewModel
{
    public override string Title => "Palette";
    public override IReadOnlyList<string> Subpages { get; } = ["Semantics", "Colors"];
}
