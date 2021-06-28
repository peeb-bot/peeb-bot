using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Peeb.Bot.Clients.Discord;
using Peeb.Bot.Clients.Discord.Caches;
using Peeb.Bot.Clients.Discord.Handlers;
using Peeb.Bot.Clients.Discord.Services;
using Peeb.Bot.Results.Character;
using Peeb.Bot.Results.Execute;
using Peeb.Bot.Results.Ok;
using Peeb.Bot.Results.Unsuccessful;
using Peeb.Bot.Settings;

namespace Peeb.Bot.Startup
{
    public static class DiscordStartup
    {
        public static IServiceCollection AddDiscord(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddScoped<ICommandHandler, LoggingHandler>()
                .AddScoped<ICommandHandler, TypingNotifierHandler>()
                .AddScoped<ICommandHandler, WorkingNotifierHandler>()
                .AddScoped<IResultHandler<ExecuteResult>, ExecuteResultHandler>()
                .AddScoped<IResultHandler<IAmResult>, IAmResultHandler>()
                .AddScoped<IResultHandler<OkResult>, OkResultHandler>()
                .AddScoped<IResultHandler<UnsuccessfulResult>, UnsuccessfulResultHandler>()
                .AddSingleton<ICommandService, CommandServiceWrapper>()
                .AddSingleton<IDiscordSocketClient, DiscordSocketClientWrapper>()
                .AddSingleton<IMessageHandler, MessageHandler>()
                .AddSingleton<IServiceScopeCache, ServiceScopeCache>()
                .Configure<DiscordSettings>(configuration.GetSection("Discord"));
        }
    }
}
