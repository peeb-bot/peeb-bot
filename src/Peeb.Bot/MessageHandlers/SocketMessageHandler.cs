using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Peeb.Bot.Discord.Commands;
using Peeb.Bot.Discord.WebSocket;

namespace Peeb.Bot.MessageHandlers
{
    public class SocketMessageHandler : ISocketMessageHandler
    {
        private readonly ICommandService _commandService;
        private readonly IConfiguration _configuration;
        private readonly IDiscordSocketClient _discordSocketClient;
        private readonly IServiceProvider _serviceProvider;

        public SocketMessageHandler(ICommandService commandService, IConfiguration configuration, IDiscordSocketClient discordSocketClient, IServiceProvider serviceProvider)
        {
            _commandService = commandService;
            _configuration = configuration;
            _discordSocketClient = discordSocketClient;
            _serviceProvider = serviceProvider;
        }

        public async Task MessageReceived(IMessage message)
        {
            var argPos = 0;

            if (message is IUserMessage { Source: MessageSource.User } userMessage && userMessage.HasStringPrefix(_configuration["CommandPrefix"], ref argPos))
            {
                var commandContext = new CommandContext(_discordSocketClient, userMessage);
                var result = await _commandService.ExecuteAsync(commandContext, argPos, _serviceProvider);

                if (result.Error.HasValue)
                {
                    await userMessage.Channel.SendMessageAsync(result.ErrorReason, messageReference: new MessageReference(message.Id));
                }
            }
        }
    }
}
