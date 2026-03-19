using System.Collections.Generic;

namespace Ink.Demo.ViewModels;

public class TokensViewModel : DemoPageViewModel
{
    public override string Title => "Tokens";

    public override IReadOnlyList<string> Subpages { get; } = ["Metrics", "Motion", "Effects"];
}
