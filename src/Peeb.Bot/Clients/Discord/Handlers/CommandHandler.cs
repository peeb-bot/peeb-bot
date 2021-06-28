using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Peeb.Bot.Clients.Discord.Handlers
{
    public abstract class CommandHandler : ICommandHandler
    {
        Task ICommandHandler.CommandExecuting(ICommandContext commandContext)
        {
            return CommandExecuting(commandContext);
        }

        Task ICommandHandler.ResultExecuting(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result)
        {
            return ResultExecuting(commandInfo, commandContext, result);
        }

        Task ICommandHandler.CommandExecuted(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result)
        {
            return CommandExecuted(commandInfo, commandContext, result);
        }

        protected virtual Task CommandExecuting(ICommandContext commandContext)
        {
            return Task.CompletedTask;
        }

        protected virtual Task ResultExecuting(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result)
        {
            return Task.CompletedTask;
        }

        protected virtual Task CommandExecuted(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result)
        {
            return Task.CompletedTask;
        }
    }
}
