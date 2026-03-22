using System.Collections.Generic;

namespace Ink.Demo.ViewModels;

public class IconsViewModel : DemoPageViewModel
{
    public override string Title => "Icons";

    public override IReadOnlyList<string> Subpages { get; } = ["Free Fonts", "Pro Faces"];
}
