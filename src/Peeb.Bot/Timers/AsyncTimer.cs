using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Peeb.Bot.Timers
{
    public class AsyncTimer : IAsyncTimer
    {
        public bool Elapsed { get; private set; }

        private readonly object _lock = new();
        private readonly TaskCompletionSource _stopCompletionSource = new();
        private readonly ITimer _timer;
        private readonly ILogger<AsyncTimer> _logger;
        private int _callbacks;
        private bool _stop;

        public AsyncTimer(ITimer timer, ILogger<AsyncTimer> logger)
        {
            _timer = timer;
            _logger = logger;
        }

        public void Start(Func<Task> callback, TimeSpan dueTime, TimeSpan period)
        {
            _timer.Start(async () =>
                {
                    Elapsed = true;

                    lock (_lock)
                    {
                        _callbacks++;
                    }

                    try
                    {
                        await callback();
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, "Timer callback failed");
                    }

                    lock (_lock)
                    {
                        _callbacks--;

                        if (_stop && _callbacks == 0)
                        {
                            _stopCompletionSource.SetResult();
                        }
                    }
                },
                dueTime,
                period);
        }

        public async Task Stop()
        {
            await _timer.Stop();

            lock (_lock)
            {
                _stop = true;

                if (_callbacks == 0)
                {
                    _stopCompletionSource.SetResult();
                }
            }

            await _stopCompletionSource.Task;
        }
    }
}
