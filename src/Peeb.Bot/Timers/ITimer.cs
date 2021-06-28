using System;

namespace Peeb.Bot.Timers
{
    public interface ITimer : IAsyncDisposable
    {
        bool Elapsed { get; }
    }
}
