using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Peeb.Bot.Discord.Commands;
using Peeb.Bot.Discord.WebSocket;
using Peeb.Bot.MessageHandlers;

namespace Peeb.Bot.HostedServices
{
    public class BotHostedService : IHostedService
    {
        private readonly ICommandService _commandService;
        private readonly IConfiguration _configuration;
        private readonly IDiscordSocketClient _discordSocketClient;
        private readonly ILogger<BotHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISocketMessageHandler _socketMessageHandler;

        public BotHostedService(
            ICommandService commandService,
            IConfiguration configuration,
            IDiscordSocketClient discordSocketClient,
            ILogger<BotHostedService> logger,
            IServiceProvider serviceProvider,
            ISocketMessageHandler socketMessageHandler)
        {
            _commandService = commandService;
            _configuration = configuration;
            _discordSocketClient = discordSocketClient;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _socketMessageHandler = socketMessageHandler;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting bot hosted service");

            _discordSocketClient.MessageReceived += _socketMessageHandler.MessageReceived;
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
            await _discordSocketClient.LoginAsync(TokenType.Bot, _configuration["DISCORD_BOT_TOKEN"]);
            await _discordSocketClient.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping bot hosted service");

            await _discordSocketClient.StopAsync();
        }
    }
}
