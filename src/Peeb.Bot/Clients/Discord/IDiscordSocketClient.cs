using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Peeb.Bot.Clients.Discord
{
    public interface IDiscordSocketClient : IDiscordClient
    {
        event Func<SocketMessage, Task> MessageReceived;
        Task LoginAsync(TokenType tokenType, string token, bool validateToken = true);
    }
}
