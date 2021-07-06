using System;

namespace Peeb.Bot.Timers
{
    public interface ITimerFactory
    {
        ITimer CreateTimer(Action callback, TimeSpan dueTime, TimeSpan period);
    }
}
