using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peeb.Bot.Models;

namespace Peeb.Bot.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.HasPartitionKey(u => u.Id);
            builder.Property(u => u.Id).HasConversion<string>();
            builder.HasMany(u => u.Characters).WithOne(c => c.User).HasForeignKey(c => c.UserId).IsRequired();
            builder.ToContainer("Users");
        }
    }
}
