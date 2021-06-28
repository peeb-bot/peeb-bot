using Microsoft.EntityFrameworkCore;
using Peeb.Bot.Data.Configurations;
using Peeb.Bot.Models;

namespace Peeb.Bot.Data
{
    public class PeebDbContext : DbContext
    {
        public DbSet<AddCharacterRoleSetting> AddCharacterRoleSettings { get; set; }
        public DbSet<AddCrossWorldLinkshellRoleSetting> AddCrossWorldLinkshellRoleSettings { get; set; }
        public DbSet<ChangeNicknameToCharacterNameSetting> ChangeNicknameToCharacterNameSettings { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<User> Users { get; set; }

        public PeebDbContext(DbContextOptions<PeebDbContext> options)
            : base(options)
        {
        }

        protected PeebDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AddCharacterRoleSettingConfiguration());
            modelBuilder.ApplyConfiguration(new AddedCrossWorldLinkshellRoleSettingConfiguration());
            modelBuilder.ApplyConfiguration(new ChangeNicknameToCharacterNameSettingConfiguration());
            modelBuilder.ApplyConfiguration(new CharacterConfiguration());
            modelBuilder.ApplyConfiguration(new SettingConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
