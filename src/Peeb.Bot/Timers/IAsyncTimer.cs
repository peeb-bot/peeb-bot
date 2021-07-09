using System;
using System.Threading.Tasks;

namespace Peeb.Bot.Timers
{
    public interface IAsyncTimer
    {
        bool Elapsed { get; }
        void Start(Func<Task> callback, TimeSpan dueTime, TimeSpan period);
        Task Stop();
    }
}
