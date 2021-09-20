using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Peeb.Bot.Data;
using Peeb.Bot.Options;

namespace Peeb.Bot.Startup
{
    public static class DatabaseStartup
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            void OptionsAction(IServiceProvider p, DbContextOptionsBuilder o)
            {
                var connectionString = configuration.GetConnectionString("Peeb");
                var options = p.GetRequiredService<IOptionsMonitor<DatabaseOptions>>();
                var databaseName = options.CurrentValue.Name;

                o.UseCosmos(connectionString, databaseName);
            }

            return services
                .AddDbContextFactory<PeebDbContext>(OptionsAction)
                .AddDbContext<PeebDbContext>(OptionsAction)
                .Configure<DatabaseOptions>(configuration.GetSection("Database"));
        }
    }
}
