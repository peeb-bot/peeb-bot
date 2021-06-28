using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Peeb.Bot.Clients.Discord.Extensions;

namespace Peeb.Bot.Clients.Discord
{
    public class DiscordSocketClientWrapper : DiscordSocketClient, IDiscordSocketClient
    {
        public DiscordSocketClientWrapper(ILogger<DiscordSocketClient> logger)
        {
            Log += logger.Log;
        }
    }
}
