using Microsoft.Extensions.DependencyInjection;
using Peeb.Bot.MessageHandlers;

namespace Peeb.Bot.Startup
{
    public static class MessageHandlersStartup
    {
        public static IServiceCollection AddMessageHandlers(this IServiceCollection services)
        {
            return services.AddSingleton<ISocketMessageHandler, SocketMessageHandler>();
        }
    }
}
