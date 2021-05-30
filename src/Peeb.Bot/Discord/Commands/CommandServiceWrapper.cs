using Discord.Commands;
using Microsoft.Extensions.Logging;
using Peeb.Bot.Extensions;

namespace Peeb.Bot.Discord.Commands
{
    public class CommandServiceWrapper : CommandService, ICommandService
    {
        public CommandServiceWrapper(ILogger<CommandService> logger)
        {
            Log += logger.Log;
        }
    }
}
