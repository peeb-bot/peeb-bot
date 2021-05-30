using Microsoft.Extensions.DependencyInjection;
using Peeb.Bot.Discord.Commands;
using Peeb.Bot.Discord.WebSocket;

namespace Peeb.Bot.Startup
{
    public static class DiscordStartup
    {
        public static IServiceCollection AddDiscord(this IServiceCollection services)
        {
            return services
                .AddSingleton<ICommandService, CommandServiceWrapper>()
                .AddSingleton<IDiscordSocketClient, DiscordSocketClientWrapper>();
        }
    }
}
