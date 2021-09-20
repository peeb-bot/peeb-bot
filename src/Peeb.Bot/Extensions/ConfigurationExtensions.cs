using Microsoft.Extensions.Configuration;

namespace Peeb.Bot.Extensions
{
    public static class ConfigurationExtensions
    {
        public static string GetDatabaseName(this IConfiguration configuration, string name)
        {
            return configuration?.GetSection("DatabaseNames")?[name];
        }
    }
}
