using System;
using System.Threading;
using System.Threading.Tasks;

namespace Peeb.Bot.Timers
{
    public class TimerWrapper : ITimer
    {
        public bool Elapsed { get; private set; }

        private readonly Timer _timer;

        public TimerWrapper(Action callback, TimeSpan dueTime, TimeSpan period)
        {
            _timer = new Timer(
                _ =>
                {
                    Elapsed = true;

                    callback();
                },
                null,
                dueTime,
                 period);
        }

        public ValueTask DisposeAsync()
        {
            return _timer.DisposeAsync();
        }
    }
}
