using System.Collections.Generic;

namespace Ink.Demo.ViewModels;

public class CalendarViewModel : DemoPageViewModel
{
    public override string Title => "Calendar";
    public override IReadOnlyList<string> Subpages { get; } = ["Ink", "Avalonia"];
}
