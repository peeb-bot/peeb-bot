using Microsoft.Extensions.DependencyInjection;

namespace Peeb.Bot.Startup
{
    public static class AutoMapperStartup
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            return services.AddAutoMapper(typeof(AutoMapperStartup));
        }
    }
}
