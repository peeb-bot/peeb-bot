using System.Threading.Tasks;
using Discord;

namespace Peeb.Bot.MessageHandlers
{
    public interface ISocketMessageHandler
    {
        Task MessageReceived(IMessage message);
    }
}
