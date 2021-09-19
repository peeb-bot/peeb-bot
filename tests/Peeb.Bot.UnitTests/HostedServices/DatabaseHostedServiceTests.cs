using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Data;
using Peeb.Bot.HostedServices;

namespace Peeb.Bot.UnitTests.HostedServices
{
    [TestFixture]
    [Parallelizable]
    public class DatabaseHostedServiceTests : TestBase<DatabaseHostedServiceTestsContext>
    {
        [Test]
        public Task StartAsync_ShouldCreateDatabase()
        {
            return TestAsync(
                c => c.StartAsync(),
                c => c.Database.Verify(d => d.EnsureCreatedAsync(CancellationToken.None), Times.Once));
        }
    }

    public class DatabaseHostedServiceTestsContext
    {
        public Mock<DatabaseFacade> Database { get; set; }
        public Mock<PeebDbContext> Db { get; set; }
        public Mock<IDbContextFactory<PeebDbContext>> DbContextFactory { get; set; }
        public DatabaseHostedService HostedService { get; set; }

        public DatabaseHostedServiceTestsContext()
        {
            Db = new Mock<PeebDbContext>();
            Database = new Mock<DatabaseFacade>(Db.Object);
            DbContextFactory = new Mock<IDbContextFactory<PeebDbContext>>();

            Db.SetupGet(d => d.Database).Returns(Database.Object);
            DbContextFactory.Setup(f => f.CreateDbContext()).Returns(Db.Object);

            HostedService = new DatabaseHostedService(DbContextFactory.Object);
        }

        public Task StartAsync()
        {
            return HostedService.StartAsync(CancellationToken.None);
        }
    }
}
