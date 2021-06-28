using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Peeb.Bot.Clients.Discord.Handlers
{
    public class TypingNotifierHandler : CommandHandler
    {
        private IDisposable _typingNotifier;

        protected override Task CommandExecuting(ICommandContext commandContext)
        {
            _typingNotifier = commandContext.Channel.EnterTypingState();

            return Task.CompletedTask;
        }

        protected override Task ResultExecuting(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result)
        {
            using (_typingNotifier)
            {
                return Task.CompletedTask;
            }
        }
    }
}
