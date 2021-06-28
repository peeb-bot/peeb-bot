using System.Threading.Tasks;

namespace Peeb.Bot.Services
{
    public interface ITaskService
    {
        Task Delay(int millisecondsDelay);
    }
}
