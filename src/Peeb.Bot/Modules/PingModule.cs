using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Peeb.Bot.Services;

namespace Peeb.Bot.Modules
{
    public class PingModule : ModuleBase<ICommandContext>
    {
        private readonly IDateTimeOffsetService _dateTimeOffsetService;

        public PingModule(IDateTimeOffsetService dateTimeOffsetService)
        {
            _dateTimeOffsetService = dateTimeOffsetService;
        }

        [Command("ping")]
        public Task Ping()
        {
            return ReplyAsync(
                $"Pong! Responded in {_dateTimeOffsetService.UtcNow.Subtract(Context.Message.Timestamp).TotalSeconds:0.000}s",
                messageReference: new MessageReference(Context.Message.Id));
        }
    }
}
