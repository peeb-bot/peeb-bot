using System;

namespace Peeb.Bot.Timers
{
    public interface ITimerFactory
    {
        ITimer CreateTimer(TimeSpan dueTime, Action callback);
    }
}
