using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Peeb.Bot.Settings;

namespace Peeb.Bot.Startup
{
    public static class SettingsStartup
    {
        public static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
        {
            return services.Configure<DiscordSettings>(configuration.GetSection("Discord"));
        }
    }
}
