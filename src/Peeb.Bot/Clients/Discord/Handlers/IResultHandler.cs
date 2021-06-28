using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Peeb.Bot.Clients.Discord.Handlers
{
    public interface IResultHandler<T> : IResultHandler where T : IResult
    {
        Task Handle(Optional<CommandInfo> commandInfo, ICommandContext commandContext, T result);
    }

    public interface IResultHandler
    {
        Task Handle(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result);
    }
}
