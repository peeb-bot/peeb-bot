using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Peeb.Bot.Clients.Discord.Caches
{
    public class ServiceScopeCache : IServiceScopeCache
    {
        private static readonly AsyncLocal<IServiceScope> Cache = new();

        public void Set(IServiceScope serviceScope)
        {
            Cache.Value = serviceScope;
        }

        public IServiceScope Get()
        {
            return Cache.Value;
        }
    }
}
