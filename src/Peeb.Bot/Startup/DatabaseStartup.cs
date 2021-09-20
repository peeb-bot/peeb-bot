using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Peeb.Bot.Data;
using Peeb.Bot.Extensions;

namespace Peeb.Bot.Startup
{
    public static class DatabaseStartup
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            void OptionsAction(IServiceProvider p, DbContextOptionsBuilder o)
            {
                var connectionString = configuration.GetConnectionString("Peeb");
                var databaseName = configuration.GetDatabaseName("Peeb");

                o.UseCosmos(connectionString, databaseName);
            }

            return services
                .AddDbContextFactory<PeebDbContext>(OptionsAction)
                .AddDbContext<PeebDbContext>(OptionsAction);
        }
    }
}
