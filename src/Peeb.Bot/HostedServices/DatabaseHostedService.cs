using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Peeb.Bot.Data;

namespace Peeb.Bot.HostedServices
{
    public class DatabaseHostedService : IHostedService
    {
        private readonly IDbContextFactory<PeebDbContext> _dbContextFactory;

        public DatabaseHostedService(IDbContextFactory<PeebDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await using var db = _dbContextFactory.CreateDbContext();
            await db.Database.EnsureCreatedAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
