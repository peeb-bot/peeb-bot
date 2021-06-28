using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peeb.Bot.Models;

namespace Peeb.Bot.Data.Configurations
{
    public class AddCharacterRoleSettingConfiguration : IEntityTypeConfiguration<AddCharacterRoleSetting>
    {
        public void Configure(EntityTypeBuilder<AddCharacterRoleSetting> builder)
        {
            builder.HasBaseType<Setting>();
            builder.HasPartitionKey(s => s.GuildId);

            builder.HasData(
                new AddCharacterRoleSetting(
                    376090072627281920,
                    846827823905767454,
                    true,
                    DateTimeOffset.UtcNow));
        }
    }
}
