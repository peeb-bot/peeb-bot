using System;

namespace Peeb.Bot.Timers
{
    public class TimerFactory : ITimerFactory
    {
        public ITimer CreateTimer(Action callback, TimeSpan dueTime, TimeSpan period)
        {
            return new TimerWrapper(callback, dueTime, period);
        }
    }
}
