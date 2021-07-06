using Microsoft.Extensions.DependencyInjection;
using Peeb.Bot.HostedServices;

namespace Peeb.Bot.Startup
{
    public static class HostedServicesStartup
    {
        public static IServiceCollection AddHostedServices(this IServiceCollection services)
        {
            return services
                .AddHostedService<PeebHostedService>()
                .AddHostedService<StatusHostedService>();
        }
    }
}
