using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Peeb.Bot.Discord.WebSocket
{
    public interface IDiscordSocketClient : IDiscordClient
    {
        event Func<SocketMessage, Task> MessageReceived;
        Task LoginAsync(TokenType tokenType, string token, bool validateToken = true);
    }
}
