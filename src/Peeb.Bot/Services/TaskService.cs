using System.Threading.Tasks;

namespace Peeb.Bot.Services
{
    public class TaskService : ITaskService
    {
        public Task Delay(int millisecondsDelay)
        {
            return Task.Delay(millisecondsDelay);
        }
    }
}
