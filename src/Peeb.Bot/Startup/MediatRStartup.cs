using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Peeb.Bot.Startup
{
    public static class MediatRStartup
    {
        public static IServiceCollection AddMediatR(this IServiceCollection services)
        {
            return services.AddMediatR(typeof(MediatRStartup));
        }
    }
}
