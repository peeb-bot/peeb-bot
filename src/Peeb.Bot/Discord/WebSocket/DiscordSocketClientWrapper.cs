using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Peeb.Bot.Extensions;

namespace Peeb.Bot.Discord.WebSocket
{
    public class DiscordSocketClientWrapper : DiscordSocketClient, IDiscordSocketClient
    {
        public DiscordSocketClientWrapper(ILogger<DiscordSocketClient> logger)
        {
            Log += logger.Log;
        }
    }
}
