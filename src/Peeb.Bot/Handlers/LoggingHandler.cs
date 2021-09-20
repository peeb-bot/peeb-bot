using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using Peeb.Bot.Clients.Discord.Handlers;
using Peeb.Bot.Extensions;
using Peeb.Bot.Services;

namespace Peeb.Bot.Handlers
{
    public class LoggingHandler : CommandHandler
    {
        private readonly IDateTimeOffsetService _dateTimeOffsetService;
        private readonly ILogger<LoggingHandler> _logger;

        public LoggingHandler(IDateTimeOffsetService dateTimeOffsetService, ILogger<LoggingHandler> logger)
        {
            _dateTimeOffsetService = dateTimeOffsetService;
            _logger = logger;
        }

        protected override Task CommandExecuting(ICommandContext commandContext)
        {
            var stringBuilder = new StringBuilder()
                .Append("Executing ")
                .AppendCommandContext(commandContext);

            _logger.LogInformation(stringBuilder.ToString());

            return Task.CompletedTask;
        }

        protected override Task CommandExecuted(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result)
        {
            var elapsed = _dateTimeOffsetService.UtcNow.Subtract(commandContext.Message.Timestamp);

            var stringBuilder = new StringBuilder()
                .Append("Executed ")
                .AppendCommandContext(commandContext)
                .AppendFormat(" in {0:0.000}s", elapsed.TotalSeconds);

            _logger.LogInformation(stringBuilder.ToString());

            return Task.CompletedTask;
        }
    }
}
