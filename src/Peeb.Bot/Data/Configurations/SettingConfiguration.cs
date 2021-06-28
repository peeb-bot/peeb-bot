using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peeb.Bot.Models;

namespace Peeb.Bot.Data.Configurations
{
    public class SettingConfiguration : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.HasKey(s => s.Id);
            builder.HasPartitionKey(s => s.GuildId);
            builder.Property(s => s.GuildId).HasConversion<string>();
            builder.ToContainer("Settings");
        }
    }
}
