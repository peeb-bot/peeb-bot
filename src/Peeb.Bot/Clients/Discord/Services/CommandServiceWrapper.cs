using Discord.Commands;
using Microsoft.Extensions.Logging;
using Peeb.Bot.Clients.Discord.Extensions;

namespace Peeb.Bot.Clients.Discord.Services
{
    public class CommandServiceWrapper : CommandService, ICommandService
    {
        public CommandServiceWrapper(ILogger<CommandService> logger)
            : base(new CommandServiceConfig { DefaultRunMode = RunMode.Async })
        {
            Log += logger.Log;
        }
    }
}
