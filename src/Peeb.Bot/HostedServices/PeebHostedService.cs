using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Peeb.Bot.Clients.Discord;
using Peeb.Bot.Clients.Discord.Handlers;
using Peeb.Bot.Clients.Discord.Services;
using Peeb.Bot.Data;
using Peeb.Bot.Settings;

namespace Peeb.Bot.HostedServices
{
    public class PeebHostedService : IHostedService
    {
        private readonly ICommandService _commandService;
        private readonly IDbContextFactory<PeebDbContext> _dbContextFactory;
        private readonly IDiscordSocketClient _discordSocketClient;
        private readonly ILogger<PeebHostedService> _logger;
        private readonly IMessageHandler _messageHandler;
        private readonly IOptionsMonitor<DiscordSettings> _settings;
        private readonly IServiceProvider _serviceProvider;

        public PeebHostedService(
            ICommandService commandService,
            IDbContextFactory<PeebDbContext> dbContextFactory,
            IDiscordSocketClient discordSocketClient,
            ILogger<PeebHostedService> logger,
            IMessageHandler messageHandler,
            IOptionsMonitor<DiscordSettings> settings,
            IServiceProvider serviceProvider)
        {
            _commandService = commandService;
            _dbContextFactory = dbContextFactory;
            _discordSocketClient = discordSocketClient;
            _logger = logger;
            _messageHandler = messageHandler;
            _settings = settings;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting {nameof(PeebHostedService)}");

            await using (var db = _dbContextFactory.CreateDbContext())
            {
                await db.Database.EnsureCreatedAsync(cancellationToken);
            }

            _discordSocketClient.MessageReceived += _messageHandler.MessageReceived;
            _commandService.CommandExecuted += _messageHandler.CommandExecuted;

            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
            await _discordSocketClient.LoginAsync(TokenType.Bot, _settings.CurrentValue.Token);
            await _discordSocketClient.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Stopping {nameof(PeebHostedService)}");

            await _discordSocketClient.StopAsync();
        }
    }
}
