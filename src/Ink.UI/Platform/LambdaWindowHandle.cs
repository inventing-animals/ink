using System;
using System.Threading.Tasks;

namespace Ink.UI.Platform;

internal sealed class LambdaWindowHandle : IWindowHandle
{
    private readonly Action _close;
    private readonly Task   _waitTask;

    public LambdaWindowHandle(Action close, Task waitTask)
    {
        _close    = close;
        _waitTask = waitTask;
    }

    public void Close()              => _close();
    public Task WaitForCloseAsync() => _waitTask;
}
