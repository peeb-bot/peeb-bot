using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Peeb.Bot.Clients.Discord.Caches;
using Peeb.Bot.Clients.Discord.Services;
using Peeb.Bot.Settings;

namespace Peeb.Bot.Clients.Discord.Handlers
{
    public class MessageHandler : IMessageHandler
    {
        private static readonly ConcurrentDictionary<Type, Type> ResultHandlerTypes = new();

        private readonly ICommandService _commandService;
        private readonly IDiscordSocketClient _discordSocketClient;
        private readonly IOptionsMonitor<DiscordSettings> _settings;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScopeCache _serviceScopeCache;

        public MessageHandler(
            ICommandService commandService,
            IDiscordSocketClient discordSocketClient,
            IOptionsMonitor<DiscordSettings> settings,
            IServiceProvider serviceProvider,
            IServiceScopeCache serviceScopeCache)
        {
            _commandService = commandService;
            _discordSocketClient = discordSocketClient;
            _settings = settings;
            _serviceProvider = serviceProvider;
            _serviceScopeCache = serviceScopeCache;
        }

        public async Task MessageReceived(IMessage message)
        {
            if (message is not IUserMessage userMessage)
            {
                return;
            }

            if (userMessage.Source != MessageSource.User)
            {
                return;
            }

            var argPos = 0;

            if (!userMessage.HasStringPrefix(_settings.CurrentValue.Prefix, ref argPos))
            {
                return;
            }

            var commandContext = new CommandContext(_discordSocketClient, userMessage);
            var searchResult = _commandService.Search(commandContext, argPos);

            if (!searchResult.IsSuccess)
            {
                return;
            }

            var serviceScope = _serviceProvider.CreateScope();
            var commandHandlers = serviceScope.ServiceProvider.GetServices<ICommandHandler>();

            foreach (var commandHandler in commandHandlers)
            {
                await commandHandler.CommandExecuting(commandContext);
            }

            _serviceScopeCache.Set(serviceScope);
            await _commandService.ExecuteAsync(commandContext, argPos, serviceScope.ServiceProvider);
        }

        public async Task CommandExecuted(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result)
        {
            using var serviceScope = _serviceScopeCache.Get();
            var resultHandlerType = ResultHandlerTypes.GetOrAdd(result.GetType(), t => typeof(IResultHandler<>).MakeGenericType(t));
            var resultHandler = (IResultHandler)serviceScope.ServiceProvider.GetService(resultHandlerType);
            var commandHandlers = serviceScope.ServiceProvider.GetServices<ICommandHandler>().Reverse().ToList();

            foreach (var commandHandler in commandHandlers)
            {
                await commandHandler.ResultExecuting(commandInfo, commandContext, result);
            }

            if (resultHandler != null)
            {
                await resultHandler.Handle(commandInfo, commandContext, result);
            }

            foreach (var commandHandler in commandHandlers)
            {
                await commandHandler.CommandExecuted(commandInfo, commandContext, result);
            }
        }
    }
}
