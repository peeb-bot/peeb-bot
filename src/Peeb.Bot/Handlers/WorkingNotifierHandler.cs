using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Peeb.Bot.Clients.Discord.Handlers;
using Peeb.Bot.Helpers;
using Peeb.Bot.Timers;

namespace Peeb.Bot.Handlers
{
    public class WorkingNotifierHandler : CommandHandler
    {
        private readonly IAsyncTimer _timer;

        public WorkingNotifierHandler(IAsyncTimer timer)
        {
            _timer = timer;
        }

        protected override Task CommandExecuting(ICommandContext commandContext)
        {
            _timer.Start(
                () => commandContext.Message.AddReactionAsync(EmojiHelper.Clock2),
                TimeSpan.FromSeconds(3),
                Timeout.InfiniteTimeSpan);

            return Task.CompletedTask;
        }

        protected override async Task ResultExecuting(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result)
        {
            await _timer.Stop();

            if (_timer.Elapsed)
            {
                await commandContext.Message.RemoveReactionAsync(EmojiHelper.Clock2, commandContext.Client.CurrentUser);
            }
        }
    }
}
