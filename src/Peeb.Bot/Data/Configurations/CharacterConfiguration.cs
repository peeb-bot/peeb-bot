using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peeb.Bot.Models;

namespace Peeb.Bot.Data.Configurations
{
    public class CharacterConfiguration : IEntityTypeConfiguration<Character>
    {
        public void Configure(EntityTypeBuilder<Character> builder)
        {
            builder.HasKey(c => c.Id);
            builder.HasPartitionKey(c => c.UserId);
            builder.Property(c => c.UserId).HasConversion<string>();
            builder.HasOne(c => c.User).WithMany(u => u.Characters).HasForeignKey(c => c.UserId).IsRequired();
            builder.ToContainer("Characters");
            builder.HasIndex(c => new { c.UserId, c.GuildId }).IsUnique();
        }
    }
}
