using System;
using System.Threading;
using System.Threading.Tasks;

namespace Peeb.Bot.Timers
{
    public class TimerWrapper : ITimer
    {
        private Timer _timer;

        public void Start(Action callback, TimeSpan dueTime, TimeSpan period)
        {
            _timer = new Timer(_ => callback(), null, dueTime, period);
        }

        public async Task Stop()
        {
            await _timer.DisposeAsync();
        }
    }
}
