using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Peeb.Bot.Helpers;
using Peeb.Bot.Timers;

namespace Peeb.Bot.Clients.Discord.Handlers
{
    public class WorkingNotifierHandler : CommandHandler
    {
        private readonly ITimerFactory _timerFactory;
        private ITimer _timer;

        public WorkingNotifierHandler(ITimerFactory timerFactory)
        {
            _timerFactory = timerFactory;
        }

        protected override Task CommandExecuting(ICommandContext commandContext)
        {
            _timer = _timerFactory.CreateTimer(
                async () => await commandContext.Message.AddReactionAsync(EmojiHelper.Clock2),
                TimeSpan.FromSeconds(3),
                Timeout.InfiniteTimeSpan);

            return Task.CompletedTask;
        }

        protected override async Task ResultExecuting(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result)
        {
            await _timer.DisposeAsync();

            if (_timer.Elapsed)
            {
                await commandContext.Message.RemoveReactionAsync(EmojiHelper.Clock2, commandContext.Client.CurrentUser);
            }
        }
    }
}
