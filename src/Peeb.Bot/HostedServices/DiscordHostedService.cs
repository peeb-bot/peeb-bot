using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Peeb.Bot.Clients.Discord;
using Peeb.Bot.Clients.Discord.Handlers;
using Peeb.Bot.Clients.Discord.Services;
using Peeb.Bot.Options;

namespace Peeb.Bot.HostedServices
{
    public class DiscordHostedService : IHostedService
    {
        private readonly ICommandService _commandService;
        private readonly IDiscordSocketClient _discordSocketClient;
        private readonly IMessageHandler _messageHandler;
        private readonly IOptionsMonitor<DiscordOptions> _options;
        private readonly IServiceProvider _serviceProvider;

        public DiscordHostedService(
            ICommandService commandService,
            IDiscordSocketClient discordSocketClient,
            IMessageHandler messageHandler,
            IOptionsMonitor<DiscordOptions> options,
            IServiceProvider serviceProvider)
        {
            _commandService = commandService;
            _discordSocketClient = discordSocketClient;
            _messageHandler = messageHandler;
            _options = options;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.MessageReceived += _messageHandler.MessageReceived;
            _commandService.CommandExecuted += _messageHandler.CommandExecuted;

            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
            await _discordSocketClient.LoginAsync(TokenType.Bot, _options.CurrentValue.Token);
            await _discordSocketClient.StartAsync();
            await _discordSocketClient.SetGameAsync("Final Fantasy XIV");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _discordSocketClient.StopAsync();
        }
    }
}
