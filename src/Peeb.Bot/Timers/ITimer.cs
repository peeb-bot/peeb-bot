using System;
using System.Threading.Tasks;

namespace Peeb.Bot.Timers
{
    public interface ITimer
    {
        void Start(Action callback, TimeSpan dueTime, TimeSpan period);
        Task Stop();
    }
}
