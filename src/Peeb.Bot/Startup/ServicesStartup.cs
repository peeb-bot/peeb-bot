using Microsoft.Extensions.DependencyInjection;
using Peeb.Bot.Services;

namespace Peeb.Bot.Startup
{
    public static class ServicesStartup
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services
                .AddSingleton<IDateTimeOffsetService, DateTimeOffsetService>()
                .AddSingleton<ITaskService, TaskService>()
                .AddScoped<INotificationsService, NotificationsService>();
        }
    }
}
