using System.Collections.Generic;

namespace Ink.Demo.ViewModels;

public class SliderViewModel : DemoPageViewModel
{
    public override string Title => "Slider";
    public override IReadOnlyList<string> Subpages { get; } = ["Ink", "Avalonia"];
}
