using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Peeb.Bot.Clients.Discord.Handlers
{
    public interface ICommandHandler
    {
        Task CommandExecuting(ICommandContext commandContext);
        Task ResultExecuting(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result);
        Task CommandExecuted(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result);
    }
}
