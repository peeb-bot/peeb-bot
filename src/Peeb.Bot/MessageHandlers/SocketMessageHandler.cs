using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;
using Peeb.Bot.Discord.Commands;
using Peeb.Bot.Discord.WebSocket;
using Peeb.Bot.Settings;

namespace Peeb.Bot.MessageHandlers
{
    public class SocketMessageHandler : ISocketMessageHandler
    {
        private readonly ICommandService _commandService;
        private readonly IDiscordSocketClient _discordSocketClient;
        private readonly IOptionsMonitor<DiscordSettings> _settings;
        private readonly IServiceProvider _serviceProvider;

        public SocketMessageHandler(ICommandService commandService, IDiscordSocketClient discordSocketClient, IOptionsMonitor<DiscordSettings> settings, IServiceProvider serviceProvider)
        {
            _commandService = commandService;
            _discordSocketClient = discordSocketClient;
            _settings = settings;
            _serviceProvider = serviceProvider;
        }

        public async Task MessageReceived(IMessage message)
        {
            var argPos = 0;

            if (message is IUserMessage { Source: MessageSource.User } userMessage && userMessage.HasStringPrefix(_settings.CurrentValue.Prefix, ref argPos))
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
