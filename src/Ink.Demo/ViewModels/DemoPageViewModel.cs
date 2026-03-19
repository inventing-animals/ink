using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Ink.Demo.ViewModels;

public abstract partial class DemoPageViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _selectedSubpage = string.Empty;

    public abstract string Title { get; }

    public virtual IReadOnlyList<string> Subpages { get; } = [];

    public bool HasSubpages => Subpages.Count > 0;
}
