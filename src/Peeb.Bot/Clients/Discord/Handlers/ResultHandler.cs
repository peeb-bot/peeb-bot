using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Peeb.Bot.Clients.Discord.Handlers
{
    public abstract class ResultHandler<T> : IResultHandler<T> where T : IResult
    {
        Task IResultHandler.Handle(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result)
        {
            return Handle(commandInfo, commandContext, (T)result);
        }

        public abstract Task Handle(Optional<CommandInfo> commandInfo, ICommandContext commandContext, T result);
    }
}
