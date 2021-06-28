using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Peeb.Bot.Clients.Discord.Handlers
{
    public interface IMessageHandler
    {
        Task MessageReceived(IMessage message);
        Task CommandExecuted(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result);
    }
}
