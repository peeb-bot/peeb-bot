using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peeb.Bot.Models;

namespace Peeb.Bot.Data.Configurations
{
    public class AddedCrossWorldLinkshellRoleSettingConfiguration : IEntityTypeConfiguration<AddCrossWorldLinkshellRoleSetting>
    {
        public void Configure(EntityTypeBuilder<AddCrossWorldLinkshellRoleSetting> builder)
        {
            builder.HasBaseType<Setting>();
            builder.HasPartitionKey(s => s.GuildId);

            builder.HasData(
                new AddCrossWorldLinkshellRoleSetting(
                    376090072627281920,
                    "f21bcc7322d10cd83657a659967e1a55fdfa63d5",
                    859115655499087922,
                    true,
                    DateTimeOffset.UtcNow));
        }
    }
}
