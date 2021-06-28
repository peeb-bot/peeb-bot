using System;

namespace Peeb.Bot.Timers
{
    public class TimerFactory : ITimerFactory
    {
        public ITimer CreateTimer(TimeSpan dueTime, Action callback)
        {
            return new TimerWrapper(dueTime, callback);
        }
    }
}
