using System;
using CommunityToolkit.Mvvm.Input;

namespace Ink.Demo.ViewModels;

public partial class SecondaryWindowViewModel : ViewModelBase
{
    private readonly Action _close;

    public SecondaryWindowViewModel(Action close)
    {
        _close = close;
    }

    [RelayCommand]
    private void Close() => _close();
}
