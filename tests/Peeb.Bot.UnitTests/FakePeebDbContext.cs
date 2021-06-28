using System;
using Microsoft.EntityFrameworkCore;
using Peeb.Bot.Data;

namespace Peeb.Bot.UnitTests
{
    public class FakePeebDbContext : PeebDbContext
    {
        public DbSet<StubEntity> Entities { get; set; }

        public FakePeebDbContext()
            : base(new DbContextOptionsBuilder<PeebDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options)
        {
        }
    }
}
