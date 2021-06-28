using Microsoft.Extensions.DependencyInjection;

namespace Peeb.Bot.Clients.Discord.Caches
{
    public interface IServiceScopeCache
    {
        public void Set(IServiceScope serviceScope);
        public IServiceScope Get();
    }
}
